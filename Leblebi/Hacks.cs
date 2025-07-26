using SharpEXR.ColorSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Leblebi
{
    public class Hacks : MonoBehaviour
    {
        static GameManager v_GameManager            = null;
        static EntityPlayerLocal v_LocalPlayer      = null;
        static bool InfiniteStamina                 = false;
        static bool InfiniteWater                   = false;
        static bool InfiniteHealth                  = false;
        static bool InfiniteFood                    = false;
        static bool Triggerbot                      = false;
        static bool InstantItemSwitch               = false;
        static bool ZombieESP                       = false;
        static bool PlayerESP                       = false;
        static bool AnimalESP                       = false;
        static bool OtherESP                        = false;
        static bool CheckZmVisibility               = false;

        public EntityAlive ClosestEntity(EntityPlayerLocal LocalPlayer, EntityType Type)
        {
            Vector3 Position = LocalPlayer.transform.position;

            EntityAlive ClosestEntity = null;
            float Distance = 1000000.0f;
            for (int i = 0; i < v_GameManager.World.EntityAlives.Count; i++)
            {
                EntityAlive IndexEntity = v_GameManager.World.EntityAlives[i];

                if (IndexEntity.IsDead() || IndexEntity.entityType != Type)
                    continue;

                float NewDistance = Vector3.Distance(Position, IndexEntity.transform.position);
                if (NewDistance < Distance)
                {
                    Distance = NewDistance;
                    ClosestEntity = IndexEntity;
                }
            }

            return ClosestEntity;
        }

        public Vector3 FixScreenPos(Vector3 ScreenPos)
        {
            Vector3 Ret;
            Ret.x = ScreenPos.x;
            Ret.y = Screen.height - ScreenPos.y;
            Ret.z = ScreenPos.z;

            return Ret;
        }

        public void Start()
        {

        }
        public void Update()
        {

        }

        public void LoadBasics()
        {
            v_GameManager = FindObjectOfType<GameManager>();
            v_LocalPlayer = v_GameManager.World.m_LocalPlayerEntity;
        }

        public void OnGUI()
        {

                                      GUI.Label(new Rect(10, 10, 200, 20), "Leblebi by Paskal");
            InfiniteStamina         = GUI.Toggle(new Rect(10, 50, 200, 20), InfiniteStamina, "Infinite Stamina");
            InfiniteWater           = GUI.Toggle(new Rect(10, 70, 200, 20), InfiniteWater, "Infinite Water");
            InfiniteHealth          = GUI.Toggle(new Rect(10, 90, 200, 20), InfiniteHealth, "Infinite Health");
            InfiniteFood            = GUI.Toggle(new Rect(10, 110, 200, 20), InfiniteFood, "Infinite Food");
            Triggerbot              = GUI.Toggle(new Rect(10, 130, 200, 20), Triggerbot, "Triggerbot");
            InstantItemSwitch       = GUI.Toggle(new Rect(10, 150, 200, 20), InstantItemSwitch, "Instant item switch");
            ZombieESP               = GUI.Toggle(new Rect(10, 170, 200, 20), ZombieESP, "Zombie ESP");
            PlayerESP               = GUI.Toggle(new Rect(10, 190, 200, 20), PlayerESP, "Player ESP");
            AnimalESP               = GUI.Toggle(new Rect(10, 210, 200, 20), AnimalESP, "Animal ESP");
            OtherESP                = GUI.Toggle(new Rect(10, 230, 200, 20), OtherESP, "Other ESP");
            CheckZmVisibility       = GUI.Toggle(new Rect(10, 250, 200, 20), CheckZmVisibility, "Check zombie visibility");

            if (GUI.Button(new Rect(10, 250, 100, 20), "Reload Objs"))
            {
                LoadBasics();
            }

            


            if (v_LocalPlayer != null)
            {
                Inventory vLPInventory = v_LocalPlayer.inventory;

                GUI.Label(new Rect(10, 270, 200, 20), "Rotation: " + v_LocalPlayer.rotation.ToString());

                if (vLPInventory != null)
                {
                    PlayerEntityStats vLPStats = v_LocalPlayer.PlayerStats;

                    if (InfiniteStamina)
                    {
                        if (vLPStats.Stamina.m_value < vLPStats.Stamina.Max * 3.0f / 4.0f)
                        {
                            vLPStats.Stamina.m_value = vLPStats.Stamina.Max;
                            vLPStats.SendStatChangePacket(NetPackageEntityStatChanged.EnumStat.Stamina);
                        }
                    }

                    if (InfiniteWater)
                    {
                        if (vLPStats.Water.m_value < vLPStats.Water.Max * 3.0f / 4.0f)
                        {
                            vLPStats.Water.m_value = vLPStats.Water.Max;
                            vLPStats.SendStatChangePacket(NetPackageEntityStatChanged.EnumStat.Water);
                        }
                    }

                    if (InfiniteHealth)
                    {
                        if (vLPStats.Health.m_value < vLPStats.Health.Max * 3.0f / 4.0f)
                        {
                            vLPStats.Health.m_value = vLPStats.Health.Max;
                            vLPStats.SendStatChangePacket(NetPackageEntityStatChanged.EnumStat.Health);
                        }
                    }

                    if (InfiniteFood)
                    {
                        if (vLPStats.Food.m_value < vLPStats.Food.Max * 3.0f / 4.0f)
                        {
                            vLPStats.Food.m_value = vLPStats.Food.Max;
                            vLPStats.SendStatChangePacket(NetPackageEntityStatChanged.EnumStat.Food);
                        }
                    }

                    if (InstantItemSwitch)
                        vLPInventory.isSwitchingHeldItem = false;

                    if (Triggerbot)
                    {
                        var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                        if (Physics.Raycast(ray, out RaycastHit Hit))
                        {
                            int k = 0;
                            Transform transform = Hit.collider.transform;
                            while (transform != null)
                            {
                                if (transform.name.ContainsCaseInsensitive("Enemies"))
                                {
                                    float Distance = Vector3.Distance(Camera.main.transform.position, Hit.collider.transform.position);
                                    if ((vLPInventory.IsHoldingGun() && Distance < 50.0f) || Distance < 2.0f)
                                    {
                                        v_LocalPlayer.Attack(false);
                                        v_LocalPlayer.Attack(true);
                                    }
                                }

                                GUI.Label(new Rect(10, 290 + k * 20, 100, 20), "HIT: " + transform.name);
                                transform = transform.parent;
                                k++;
                            }
                        }
                    }
                }


                for (int i = 0; i < v_GameManager.World.Entities.list.Count; i++)
                {
                    Entity v_selEntity = v_GameManager.World.Entities.list[i];

                    if (v_selEntity)
                    {
                        Color DrawColor = new Color(0.0f, 0.0f, 0.0f);

                        Vector3 vec3_EHead = v_selEntity.transform.position;
                        vec3_EHead.y += (v_selEntity.GetEyeHeight() + 0.25f);
                        Vector3 vec3_EPos = v_selEntity.transform.position;
                        Vector3 v_vec3ScrEHeadPos = FixScreenPos(Camera.main.WorldToScreenPoint(vec3_EHead));
                        Vector3 v_vec3ScrEPos = FixScreenPos(Camera.main.WorldToScreenPoint(vec3_EPos));
                        if (v_vec3ScrEHeadPos.z > 0 && v_vec3ScrEPos.z > 0)
                        {
                            float Height = v_vec3ScrEPos.y - v_vec3ScrEHeadPos.y;
                            float Width = Height / 2.0f;
                            float Distance = Vector3.Distance(Camera.main.transform.position, vec3_EHead);

                            if (v_selEntity.IsAlive())
                            {
                                EntityAlive v_selEntityAlive = v_selEntity.GetComponent<EntityAlive>();

                                if (v_selEntityAlive != null)
                                {
                                    string EntityName = v_selEntityAlive.EntityName;
                                    if (EntityName.ContainsCaseInsensitive("player") || v_selEntityAlive.entityType == EntityType.Player)
                                    {
                                        if (!PlayerESP || v_selEntityAlive == v_LocalPlayer)
                                            continue;

                                        DrawColor = Color.blue;
                                    }
                                    else if (EntityName.ContainsCaseInsensitive("zombie") || v_selEntityAlive.entityType == EntityType.Zombie)
                                    {
                                        if (!ZombieESP)
                                            continue;

                                        bool bIsVisible = false;
                                        if (CheckZmVisibility)
                                        {
                                            Vector3 vec3_ZBellyPos = v_selEntityAlive.getBellyPosition();

                                            var ray = new Ray(Camera.main.transform.position, (vec3_EHead + new Vector3(0.0f, -0.25f, 0.0f)) - Camera.main.transform.position);
                                            if (Physics.Raycast(ray, out RaycastHit Hit2))
                                            {
                                                if (Hit2.collider.transform.parent.name.Contains("zombie"))
                                                {
                                                    bIsVisible = true;
                                                }
                                            }

                                            if (!bIsVisible)
                                            {
                                                ray = new Ray(Camera.main.transform.position, vec3_ZBellyPos - Camera.main.transform.position);
                                                if (Physics.Raycast(ray, out RaycastHit Hit3))
                                                {
                                                    if (Hit3.collider.transform.parent.name.Contains("zombie"))
                                                    {
                                                        bIsVisible = true;
                                                    }
                                                }
                                            }

                                            if (!bIsVisible)
                                            {
                                                ray = new Ray(Camera.main.transform.position, (vec3_EPos + new Vector3(0.0f, 0.25f, 0.0f)) - Camera.main.transform.position);
                                                if (Physics.Raycast(ray, out RaycastHit Hit4))
                                                {
                                                    if (Hit4.collider.transform.parent.name.Contains("zombie"))
                                                    {
                                                        bIsVisible = true;
                                                    }
                                                }
                                            }

                                            if (!bIsVisible)
                                            {
                                                bIsVisible = Distance < 2.5f;
                                            }
                                        }

                                        DrawColor = bIsVisible ? Color.magenta : Color.red;
                                    }
                                    else if (EntityName.ContainsCaseInsensitive("animal") || v_selEntityAlive.entityType == EntityType.Animal)
                                    {
                                        if (!AnimalESP)
                                            continue;

                                        DrawColor = Color.green;
                                    }
                                    else if (OtherESP)
                                    {
                                        //if (v_selEntityAlive.IsDead())
                                        //    continue;

                                        DrawColor = Color.yellow;
                                    }
                                    else
                                        continue;
                                }
                            }

                            
                            GUI.Label(new Rect(v_vec3ScrEPos.x - Width / 2, v_vec3ScrEHeadPos.y - 40, 300, 20), Distance.ToString());
                            GUI.Label(new Rect(v_vec3ScrEPos.x - Width / 2, v_vec3ScrEHeadPos.y - 20, 300, 20), v_selEntity.LocalizedEntityName);
                            GUIDrawer.DrawLine(new Vector2(Screen.width / 2.0f, Screen.height), new Vector2(v_vec3ScrEPos.x, v_vec3ScrEPos.y), DrawColor, 2.5f);
                            GUIDrawer.DrawQuad(new Rect(v_vec3ScrEPos.x - Width / 2, v_vec3ScrEHeadPos.y, Width, Height), DrawColor, 2.5f);
                        }
                    }
                }
            }
        }
    }
}
