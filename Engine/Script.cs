using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Engine
{
    public class Script : GameLogic
    {
        public MainWindow window;

        GameEntity wall;
        GameEntity triggerRight;
        GameEntity triggerLeft;
        GameEntity player;
        
        float speed = 0.1f;
        float playerSpeed = 1;


        public void Start()
        {
            wall = FindEntityByName("wall");
            triggerRight = FindEntityByName("triggerRight");
            triggerLeft = FindEntityByName("triggerleft");
            player = FindEntityByName("player");
        }

        public void Update()
        {
            if (CheckForMouseOverlap(triggerRight) && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                TranslateEntity(wall, new Vector(1, 0) * speed);
            }

            if(CheckForMouseOverlap(triggerLeft) && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                TranslateEntity(wall, new Vector(-1, 0) * speed);
            }

            if(Environment.GetKey(Key.Q))
            {
                playerSpeed++;
            }

            if(Environment.GetKey(Key.E))
            {
                playerSpeed--;
            }

            UpdatePlayer();
        }

        void UpdatePlayer()
        {
            if (Environment.GetKey(Key.W))
            {
                if (!player.Collision(player, new Vector(0, playerSpeed)))
                    player.position += new Vector(0, -speed);
            }
            if (Environment.GetKey(Key.S))
            {
                if (!player.Collision(player, new Vector(0, -playerSpeed)))
                    player.position += new Vector(0, speed);
            }
            if (Environment.GetKey(Key.A))
            {
                if (!player.Collision(player, new Vector(playerSpeed, 0)))
                    player.position += new Vector(-speed, 0);
            }
            if (Environment.GetKey(Key.D))
            {
                if (!player.Collision(player, new Vector(-playerSpeed, 0)))
                    player.position += new Vector(speed, 0);
            }
        }
    }
}
