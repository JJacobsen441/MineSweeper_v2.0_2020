using Accord.Neuro;
using Accord.Neuro.Learning;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MineSweeper.Ann;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace MineSweeper
{
    class Tank
    {
        int[] neurons = new int[] { 2, 30, 3 };
        private enum DOTS { RED, BLUE}
        private KeyboardState currentState;

        private Texture2D tank;
        private Vector2 position_tank;
        private Vector2 origin;
        private Vector2 vec_closest = new Vector2();
        private float[] wheels = new float[] { .5f, .5f };
        private float rotation = 90f;
        private float speed;
        private float dir_closest;

        private Texture2D pixel_black;
        private Texture2D pixel_red;
        private List<Vector2> r_pos = new List<Vector2>();
        private List<Vector2> b_pos = new List<Vector2>();
        private List<Vector2> lines = new List<Vector2>();
        private List<Vector2> line_closest = new List<Vector2>();
        //Int32[] pixel = { 0xFF0000 }; // White. 0xFF is Red, 0xFF0000 is Blue

        private SpriteFont font_wheels;

        private Ground g;
        private BNet net;

        public Tank(Ground g) 
        {
            this.g = g;
        }
        
        private void CalcRotation()
        {
            float amt = 0f;

            if(wheels[0] - wheels[1] != 0)
                amt = (wheels[0] - wheels[1]) * 2f;

            //amt = (wheels[0] + wheels[1]) / 2;
            //amt = wheels[1] == wheels[0] ? 0 : wheels[0] > wheels[1] ? 0.5f : -0.5f;

            rotation = rotation + amt;
            if (rotation >= 360f)
                rotation = rotation - 360f;
            if (rotation < 0f)
                rotation = 360f + rotation;
        }
        private Vector2 UnitVector()
        {
            float x = (float)(1 * Math.Cos(Math.PI * rotation / 180.0));
            float y = (float)(1 * Math.Sin(Math.PI * rotation / 180.0));
            return new Vector2(x, y);
        }
        private Vector2 AddDistance(Vector2 u)
        {
            //speed = (wheels[0] - wheels[1]) * 1f;
            //speed = 1f;
            //speed = ((wheels[0] + wheels[1]) / 2f) * 1f;

            //if (speed == 0.0f)
            speed = .5f;
            if (speed < 0.0f)
                speed = -speed;

            float x = u.X != 0.0f ? u.X * speed : speed;
            float y = u.Y != 0.0f ? u.Y * speed : speed;

            return new Vector2(x, y);
        }
        private void CalcPosition()
        {
            Vector2 add_vec = AddDistance(UnitVector()); 
            float new_x = (float)(position_tank.X + add_vec.X);
            float new_y = (float)(position_tank.Y + add_vec.Y);
            
            position_tank.X = new_x;
            position_tank.Y = new_y;

            if (position_tank.X >= 20 * Ground.Width)
                position_tank.X = 0f;
            if (position_tank.X < 0f)
                position_tank.X = 20 * Ground.Width;
            if (position_tank.Y >= 20 * Ground.Height)
                position_tank.Y = 0f;
            if (position_tank.Y < 0f)
                position_tank.Y = 20 * Ground.Height;
        }
        private void F_Setup(Vector2 pos, out int x_left, out int x_right, out int y_top, out int y_bottom, out float length, out float height, out bool type)
        {
            x_left = position_tank.X < pos.X ? (int)position_tank.X : (int)pos.X;
            x_right = position_tank.X < pos.X ? (int)pos.X : (int)position_tank.X;
            y_top = position_tank.Y < pos.Y ? (int)position_tank.Y : (int)pos.Y;
            y_bottom = position_tank.Y < pos.Y ? (int)pos.Y : (int)position_tank.Y;

            length = x_right - x_left;
            height = y_bottom - y_top;

            bool ta = false, tb = false;
            if (position_tank.X < pos.X && position_tank.Y < pos.Y)
                ta = true;
            else if (position_tank.X > pos.X && position_tank.Y > pos.Y)
                tb = true;

            type = ta || tb;
        }
        
        private void DrawLines(DOTS dots)
        {
            List<Vector2> v_pos = dots == DOTS.RED ? r_pos : b_pos;

            int x_left, x_right, y_top, y_bottom;
            float length, height;
            bool type;

            foreach (Vector2 pos in v_pos)
            {
                F_Setup(pos, out x_left, out x_right, out y_top, out y_bottom, out length, out height, out type);

                float scale = length != 0f ? (float)(height / length) : -1f;
                int adder = 10;

                float y = 0f;
                int xb = x_right - x_left, x = 0;
                for (int xa = 0; xa < x_right - x_left; xa += adder)
                {
                    x = type ? xa : xb;
                    y = (float)(x * scale);
                    Vector2 v = new Vector2((float)xa + x_left, (float)y + y_top);
                    if(pos.X == vec_closest.X && pos.Y == vec_closest.Y)
                        line_closest.Add(v);
                    else
                        lines.Add(v);
                    xb -= adder;
                }
            }
        }
        private Vector2 ClosestDot(DOTS dots, out float angle)
        {
            r_pos = g.RedPositions;
            List<Vector2> v_pos = dots == DOTS.RED ? r_pos : b_pos;

            Vector2 closest = new Vector2(0f, 0f);
            float hyp = 999.0f;
            int x_left, x_right, y_top, y_bottom;
            float length, height;
            bool type;

            angle = 0.0f;
            foreach (Vector2 pos in v_pos)
            {
                F_Setup(pos, out x_left, out x_right, out y_top, out y_bottom, out length, out height, out type);

                float hyp_tmp = (float)Math.Sqrt(Math.Pow(length, 2) + Math.Pow(height, 2));
                if (hyp_tmp < hyp)
                {
                    hyp = hyp_tmp;
                    closest = pos;
                    
                    AngleDot(pos, out angle);
                }
            }
            return closest;
        }

        private void AngleDot(Vector2 pos, out float angle)
        {
            float rad = (float)(Math.Atan2(pos.Y - position_tank.Y, pos.X - position_tank.X));
            angle = (float)(rad / Math.PI * 180) + (rad > 0 ? 0 : 360);
        }

        private void CalcSetup(float rot, float clo, out double[] input, out double[] target) 
        {
            input = new double[] { (rot / 360f) * (.9f - .1f) + .1f, (clo / 360f) * (.9f - .1f) + .1f };
            
            bool largerthan_r = rot > 180;
            float a = rot > 180 ? rot - 180 : rot + 180;
            float h = Math.Max(rot, a);
            float l = Math.Min(rot, a);
            float rot_diff = rot - clo;

            if ((rot_diff >= 0 && rot_diff < 5) || (rot_diff < 0 && rot_diff > -5))
                target = new double[] { .1f, .9f, .1f };
            else if ((largerthan_r && clo < h && clo > l) || (!largerthan_r && (clo > h || clo < l)))
                target = new double[] { .1f, .1f, .9f };//venstre, pres på højre
            else 
                target = new double[] { .9f, .1f, .1f };//højre, pres på venstre            
        }
        private void AdjustWheels() 
        {
            double[] _output;
            double[] input;
            double[] target;

            CalcSetup((int)rotation, (int)dir_closest, out input, out target);
            
            net.EvaluateNet(input, target, out _output);

            float hi = (float)Math.Max(_output[0], _output[2]);
            hi = (float)Math.Max(hi, (float)_output[1]);
            if (Math.Round(_output[0], 3) == Math.Round(hi, 3))//højre, pres på venstre
            {
                wheels[0] = wheels[0] + .1f;
                wheels[1] = wheels[1] - .1f;

                wheels[0] = wheels[0] > .9f ? .9f : wheels[0];
                wheels[1] = wheels[1] < .1f ? .1f : wheels[1];
            }
            else if (Math.Round(_output[2], 3) == Math.Round(hi, 3))//venstre, pres på højre
            {
                wheels[0] = wheels[0] - .1f;
                wheels[1] = wheels[1] + .1f;

                wheels[0] = wheels[0] < .1f ? .1f : wheels[0];
                wheels[1] = wheels[1] > .9f ? .9f : wheels[1];
            }
            else
            {
                wheels[0] = .5f;
                wheels[1] = .5f;
            }

            for (int i = 0; i < 2; i++)
            {
                if (wheels[i] <= .1f) wheels[i] = .1f;
                else if (wheels[i] <= .2f) wheels[i] = .2f;
                else if (wheels[i] <= .3f) wheels[i] = .3f;
                else if (wheels[i] <= .4f) wheels[i] = .4f;
                else if (wheels[i] <= .5f) wheels[i] = .5f;
                else if (wheels[i] <= .6f) wheels[i] = .6f;
                else if (wheels[i] <= .7f) wheels[i] = .7f;
                else if (wheels[i] <= .8f) wheels[i] = .8f;
                else if (wheels[i] <= .9f) wheels[i] = .9f;
                else wheels[i] = .9f;
            }
        }


        private int GetDirection(double[] t) 
        {
            int c = -1;
            double f = 0.0f;
            for (int j = 0; j < neurons[2]; j++)
            {
                if (t[j] > f)
                {
                    c = j;
                    f = t[j];
                }
            }
            return c;
        }
        private bool IsNext(double[] o, double[] n) 
        {
            switch (GetDirection(n)) 
            {
                case 0:
                    return GetDirection(o) == 2;
                case 1:
                    return GetDirection(o) == 0;
                case 2:
                    return GetDirection(o) == 1;
            }
            throw new Exception();
        } 
        private void Train() 
        {
            Random rand = new Random();
            double[] target;
            double[] input;
            
            float r = rand.Next(0, 359);
            float c = rand.Next(0, 359);

            CalcSetup(r, c, out input, out target);

            for (int i = 0; i < 100; i++)
            {
                double[] o = target;
                while (!IsNext(o, target))
                {
                    r = rand.Next(0, 359);
                    c = rand.Next(0, 359);

                    CalcSetup(r, c, out input, out target);
                } 

                net.TrainNet(input, target, 1);
            }
        }
        public void Load(GraphicsDeviceManager graphics, ContentManager Content)
        {
            // TODO: use this.Content to load your game content here
            tank = Content.Load<Texture2D>("Tank");
            position_tank = new Vector2(30 * 20f, 0 * 20f);
            origin = new Vector2(tank.Width / 2f, tank.Height / 2f);

            pixel_black = Content.Load<Texture2D>("Pixel_2_black");
            pixel_red = Content.Load<Texture2D>("Pixel_2_red");
            font_wheels = Content.Load<SpriteFont>("Game");
            
            r_pos = new List<Vector2>();
            lines = new List<Vector2>();
            line_closest = new List<Vector2>();
            r_pos = g.RedPositions;
            vec_closest = ClosestDot(DOTS.RED, out dir_closest);
            net = new BNet(neurons, 1, .25f, .9f);//neurons, slope?, learningrate, momentum
            net.Randomize();
        }
        public void Update()
        {
            // TODO: Add your update logic here
            r_pos = new List<Vector2>();
            lines = new List<Vector2>();
            line_closest = new List<Vector2>();
            r_pos = g.RedPositions;
            currentState = Keyboard.GetState();
            if (currentState.IsKeyDown(Keys.R))//højre, pres på venstre
            {
                position_tank = new Vector2(30 * 20f, 0 * 20f);
                rotation = 90f;
                wheels[0] = .5f;
                wheels[1] = .5f;

                r_pos = new List<Vector2>();
                lines = new List<Vector2>();
                line_closest = new List<Vector2>();
                r_pos = g.RedPositions;
                vec_closest = ClosestDot(DOTS.RED, out dir_closest);
                net = new BNet(neurons, 1, .25f, .9f);//neurons, slope?, learningrate, momentum
                net.Randomize();
            }
            
            if (draw)
                draw = false;
            CalcRotation();
            CalcPosition();
            DrawLines(DOTS.RED);
            if(currentState.IsKeyDown(Keys.N) || g.Collision((int)position_tank.X, (int)position_tank.Y, vec_closest))
                vec_closest = ClosestDot(DOTS.RED, out dir_closest);
            else
                AngleDot(vec_closest, out dir_closest);
        
            Train();
            AdjustWheels();
        }

        private bool draw = false;
        public void Draw(SpriteBatch spriteBatch) 
        {
            draw = true;
            spriteBatch.Draw(tank, position_tank, null, null, origin, (float)((rotation + 90f) * Math.PI) / 180, new Vector2(1f, 1f), Color.White, SpriteEffects.None, 0f);
            foreach (Vector2 v in lines)
                spriteBatch.Draw(pixel_black, v, Color.White);
            foreach (Vector2 v in line_closest)
                spriteBatch.Draw(pixel_red, v, Color.White);

            spriteBatch.DrawString(font_wheels, "[0]: " + wheels[0] + " [1]: " + wheels[1], new Vector2(26 * 20, 22 * 20), Color.White);
        }
    }
}
