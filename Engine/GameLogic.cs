using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Engine.MainWindow;

namespace Engine
{
    public class GameLogic
    {
        public static MainWindow mainWindow;

        /// <summary>
        /// Загрузка сцены по пути
        /// </summary>
        /// <param name="path">Путь к сцене</param>
        public void LoadScene(string path)
        {
            mainWindow.LoadScene(path);
        }

        /// <summary>
        /// Поиск сущности по имени
        /// </summary>
        /// <param name="name">Имя сущности</param>
        /// <returns></returns>
        public GameEntity FindEntityByName(string name)
        {
            return MainWindow.entitiesOnScene.Find(item => item.name.ToLower() == name.ToLower());
        }

        /// <summary>
        /// Передвигает сущность в заданном направлении
        /// </summary>
        /// <param name="gameEntity">сущность</param>
        /// <param name="direction">направление</param>
        public void TranslateEntity(GameEntity gameEntity, Vector direction)
        {
            gameEntity.position.X += direction.X;
            gameEntity.position.Y += direction.Y;
        }

        /// <summary>
        /// Уничтожение сущности (НЕ БЕЗОПАСНО)
        /// </summary>
        /// <param name="gameEntity">сущность</param>
        public void DestroyEntity(GameEntity gameEntity)
        {
            if(gameEntity != null)
            {
                if (gameEntity.sprite != null)
                {
                    mainWindow.MainGrid.Children.Remove(gameEntity.sprite);
                }

                MainWindow.entitiesOnScene.Remove(gameEntity);
            }
        }

        /// <summary>
        /// Установка позиции для сущности
        /// </summary>
        /// <param name="entity">сущность</param>
        /// <param name="vector">позиция</param>
        public void SetEntityPosition(GameEntity entity, Vector vector)
        {
            entity.position = vector;
        }

        /// <summary>
        /// Проверка на перекрытие сущности курсором мыши
        /// </summary>
        /// <param name="gameEntity">сущность</param>
        /// <returns>true, если курсор пыши перекрывает сущность</returns>
        public bool CheckForMouseOverlap(GameEntity gameEntity)
        {
            if(gameEntity.sprite != null)
            {
                return gameEntity.sprite.IsMouseOver;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Класс сущности
    /// </summary>
    public class GameEntity
    {
        public Rectangle sprite;
        public string name;
        public string texturePath;
        public bool anBool;
        public MainWindow.EntityTypes type;
        public Vector position;
        public Vector scale;
        public bool initialized = false;
        public EventHandler onCollide;

        public float rotation = 0;

        MainWindow _window;

        Script script = new Script();

        public List<string> behavior = new List<string>();
        public List<Variable> variables = new List<Variable>();

        public static double Angle(Point origin, Point target)
        {
            Vector vecTo = target - origin;
            vecTo.Normalize();
            double dotAngle = -vecTo.Y;
            double angle = Math.Acos(dotAngle);
            angle = angle * 180 / Math.PI;
            if (vecTo.X > 0)
                return angle;
            else
                return -angle;
        }

        public void Initialize(Entity _entity, MainWindow window)
        {
            texturePath = _entity.texturePath;
            name = _entity.name;
            position = _entity.position;
            scale = _entity.scale;
            type = (EntityTypes)_entity.type;
            behavior = _entity.behaviors;
            variables = _entity.variables;

            Rectangle rectangle = new Rectangle()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,

                Height = scale.X,
                Width = scale.Y,
                Margin = new Thickness(position.X, position.Y, 0, 0),
                Fill = new ImageBrush(new BitmapImage(new Uri(texturePath)))
            };

            sprite = rectangle;
            window.MainGrid.Children.Add(sprite);
            this._window = window;

            initialized = true;
        }

        public void Update()
        {
            if(sprite == null)
            {
                return;
            }

            sprite.Margin = new Thickness(position.X, position.Y, 0, 0);

            if (behavior.Contains("Look"))
            {
                RotateTransform rotateTransform = new RotateTransform(Angle(new Point(sprite.Margin.Left + (sprite.Width / 2),
                sprite.Margin.Top + (sprite.Height / 2)), Mouse.GetPosition(_window.MainGrid)) - 90, sprite.Width / 2, sprite.Height / 2);
                rotation = (float)Angle(new Point(sprite.Margin.Left + (sprite.Width / 2), sprite.Margin.Top + (sprite.Height / 2)),
                Mouse.GetPosition(_window.MainGrid)) - 90;
                sprite.RenderTransform = rotateTransform;
            }
            script.window = _window;
        }

        public bool Collision(GameEntity instance, Vector move)
        {
            bool collide = false;

            for (int i = 0; i < MainWindow.entitiesOnScene.Count; i++)
            {
                if (MainWindow.entitiesOnScene[i].sprite != instance.sprite)
                {
                    if (instance.behavior.Contains("Player") || instance.behavior.Contains("Solid") || instance.behavior.Contains("Trigger"))
                    {
                        var spriteC = MainWindow.entitiesOnScene[i].sprite;

                        if (spriteC != null)
                        {
                            collide = new Rect(sprite.Margin.Left, sprite.Margin.Top, sprite.Width, sprite.Height).IntersectsWith(new Rect(spriteC.Margin.Left + move.X, spriteC.Margin.Top + move.Y, spriteC.Width, spriteC.Height));
                        }

                        if (collide)
                        {
                            instance.onCollide?.Invoke(this, null);

                            if (MainWindow.entitiesOnScene[i].behavior.Contains("Trigger"))
                            {
                                MainWindow.entitiesOnScene[i].onCollide?.Invoke(this, null);
                                return false;
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    public class Scene
    {
        public List<Entity> entities;

        //Serialization
        public Scene()
        {

        }
    }

    public class Entity
    {
        public string name;
        public string texturePath;
        public bool anBool;
        public int type;
        public Vector position;
        public Vector scale;

        public List<Variable> variables = new List<Variable>();
        public List<string> behaviors = new List<string>();

        public Entity()
        {

        }
    }

    public class Variable
    {
        public string strv = null;
        public int intv;
        public string name;

        public Variable()
        {

        }
        public Variable(string _name, string _str)
        {
            this.strv = _str;
            this.name = _name;
        }
        public Variable(string _name, int _int)
        {
            this.intv = _int;
            this.name = _name;
        }
    }
}
