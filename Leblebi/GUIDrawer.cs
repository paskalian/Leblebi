using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Leblebi
{
    public class GUIDrawer : MonoBehaviour
    {
        private static Texture2D _lineTex;

        // Call this inside OnGUI to draw a line
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width = 1f)
        {
            if (_lineTex == null)
            {
                _lineTex = new Texture2D(1, 1);
                _lineTex.SetPixel(0, 0, Color.white);
                _lineTex.Apply();
            }

            // Calculate angle and length
            Vector2 delta = pointB - pointA;
            float angle = Mathf.Rad2Deg * Mathf.Atan2(delta.y, delta.x);
            float length = delta.magnitude;

            // Save GUI matrix and rotate
            Matrix4x4 matrixBackup = GUI.matrix;

            GUI.color = color;

            // Pivot at pointA and rotate by angle
            GUIUtility.RotateAroundPivot(angle, pointA);

            // Draw the texture as a thin rectangle stretched to the length of the line
            GUI.DrawTexture(new Rect(pointA.x, pointA.y - (width / 2), length, width), _lineTex);

            // Restore GUI matrix and color
            GUI.matrix = matrixBackup;
            GUI.color = Color.white;
        }

        public static void DrawFilledQuad(Rect position, Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            GUI.skin.box.normal.background = texture;
            GUI.Box(position, GUIContent.none);
        }

        public static void DrawQuad(Rect position, Color color, float Thickness)
        {
            DrawLine(new Vector2(position.xMin, position.yMin), new Vector2(position.xMin, position.yMax), color, Thickness);
            DrawLine(new Vector2(position.xMin, position.yMin), new Vector2(position.xMax, position.yMin), color, Thickness);
            DrawLine(new Vector2(position.xMax, position.yMax), new Vector2(position.xMin, position.yMax), color, Thickness);
            DrawLine(new Vector2(position.xMax, position.yMax), new Vector2(position.xMax, position.yMin), color, Thickness);
        }
    }
}
