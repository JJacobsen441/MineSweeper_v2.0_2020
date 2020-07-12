using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MineSweeper
{
    class Ground
    {
        public static int Width { get { return 32; } }
        public static int Height { get { return 24; } }

        Texture2D tile;
        Vector2 position_ground;
        Texture2D red;
        Texture2D blue;

        List<Vector2> red_dots = null;
        string[,] ground = new string [,] {
            {"r", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "r", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "r", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"},
            {"g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g"}};

        public List<Vector2> RedPositions
        {
            get
            {
                if (red_dots == null)
                {
                    List<Vector2> res = new List<Vector2>();
                    for (int i = 0; i < Height; i++)
                    {
                        for (int j = 0; j < Width; j++)
                        {
                            if (ground[i, j] == "r")
                                res.Add(new Vector2((float)j * 20 + 10, (float)i * 20 + 10));
                        }
                    }
                    red_dots = res;
                }
                return red_dots;
            }
        }
        Vector2 last_collision = new Vector2(-1, -1);
        public bool Collision(int x, int y, Vector2 closest) 
        {
            int x_norm = x / 20;
            int y_norm = y / 20;
            if (x_norm != last_collision.X || y_norm != last_collision.Y) 
            {
                foreach (Vector2 v in RedPositions) 
                {
                    if (x_norm == (int)(v.X / 20) && y_norm == (int)(v.Y / 20) && v.X == closest.X && v.Y == closest.Y)
                    {
                        Random r = new Random();
                        int x_n = r.Next(0, Width);
                        int y_n = r.Next(0, Height);
                        ground[y_n, x_n] = "r";
                        ground[y_norm, x_norm] = "g";
                        red_dots = null;
                        last_collision = new Vector2(x_norm, y_norm);
                        return true;
                    }
                }
            }
            return false;
        }

        public void Load(ContentManager Content) 
        {
            tile = Content.Load<Texture2D>("Ground");
            position_ground = new Vector2(0f, 0f);

            red = Content.Load<Texture2D>("Red");
            //position_red = new Vector2(40f, 0f);
            blue = Content.Load<Texture2D>("Blue");
            //position_blue = new Vector2(60f, 0f);

            red_dots = RedPositions;
        }
        public void Draw(SpriteBatch spriteBatch) 
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    string s = ground[i,j];
                    if (s == "g")
                        spriteBatch.Draw(tile, position_ground, Color.White);
                    else if (s == "r")
                        spriteBatch.Draw(red, position_ground, Color.White);
                    else if (s == "b")
                        spriteBatch.Draw(blue, position_ground, Color.White);
                    position_ground.X += 20f;
                }
                position_ground.X = 0f;
                position_ground.Y += 20f;
            }
            position_ground.Y = 0f;
            //return spriteBatch;
        }        
    }
}
