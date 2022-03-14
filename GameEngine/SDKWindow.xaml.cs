using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace GameEngine
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Entity> entitiesOnScene = new List<Entity>();
        public List<Variable> tmp_variables_list = new List<Variable>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var behavior = new List<string>();

            for (int i = 0; i < BehaviorsListBox.Items.Count; i++)
            {
                behavior.Add(BehaviorsListBox.Items[i] as string);
            }

            entitiesOnScene.Add(new Entity
            {
                name = NameBox.Text,
                texturePath = TexturePath.Text,
                anBool = (bool)BoolCheckButton.IsChecked,
                type = TypeBox.SelectedIndex,
                position = new Vector(int.Parse(XPosBox.Text), int.Parse(YPosBox.Text)),
                scale = new Vector(int.Parse(XScaleBox.Text), int.Parse(YScaleBox.Text)),
                behaviors = behavior,
                variables = tmp_variables_list
            });
            UpdateList();
        }

        void UpdateList()
        {
            EntitiesBox.Items.Clear();

            for (int i = 0; i < entitiesOnScene.Count; i++)
            {
                EntitiesBox.Items.Add(entitiesOnScene[i].name);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if(EntitiesBox.SelectedIndex != -1)
            {
                NameBox.Text = entitiesOnScene[EntitiesBox.SelectedIndex].name;
                TexturePath.Text = entitiesOnScene[EntitiesBox.SelectedIndex].texturePath;
                BoolCheckButton.IsChecked = entitiesOnScene[EntitiesBox.SelectedIndex].anBool;
                TypeBox.SelectedIndex = entitiesOnScene[EntitiesBox.SelectedIndex].type;
                XPosBox.Text = entitiesOnScene[EntitiesBox.SelectedIndex].position.X.ToString();
                YPosBox.Text = entitiesOnScene[EntitiesBox.SelectedIndex].position.Y.ToString();

                XScaleBox.Text = entitiesOnScene[EntitiesBox.SelectedIndex].scale.X.ToString();
                YScaleBox.Text = entitiesOnScene[EntitiesBox.SelectedIndex].scale.Y.ToString();

                BehaviorsListBox.Items.Clear();

                for (int i = 0; i < entitiesOnScene[EntitiesBox.SelectedIndex].behaviors.Count; i++)
                {
                    BehaviorsListBox.Items.Add(entitiesOnScene[EntitiesBox.SelectedIndex].behaviors[i]);
                }

                tmp_variables_list = entitiesOnScene[EntitiesBox.SelectedIndex].variables;
                UpdateVariablesList();
            }
        }

        private void SerializeButton_Click(object sender, RoutedEventArgs e)
        {
            XmlSerializer s = new XmlSerializer(typeof(Scene));
            TextWriter writer = new StreamWriter(System.IO.Path.GetFullPath(ScenePath.Text));
            Scene scene = new Scene();

            scene.entities = entitiesOnScene;

            s.Serialize(writer, scene);
            writer.Close();
        }

        private void SceneLoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            if(openFileDialog.FileName == "")
            {
                return;
            }

            
            if(File.Exists(openFileDialog.FileName))
            {
                entitiesOnScene.Clear();
                
                XmlSerializer s = new XmlSerializer(typeof(Scene));

                using (Stream reader = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    Scene scene = (Scene)s.Deserialize(reader);

                    for (int i = 0; i < scene.entities.Count; i++)
                    {
                        entitiesOnScene.Add(scene.entities[i]);
                    }
                }
            }

            UpdateList();
        }

        private void DeleteEntityButton_Click(object sender, RoutedEventArgs e)
        {
            if(EntitiesBox.SelectedIndex != -1)
            {
                entitiesOnScene.RemoveAt(EntitiesBox.SelectedIndex);
                UpdateList();
            }
        }

        private void AddBehaviorButton_Click(object sender, RoutedEventArgs e)
        {
            BehaviorsListBox.Items.Add(BehaviorNameBox.Text);
        }

        private void RemoveBehaviorButton_Click(object sender, RoutedEventArgs e)
        {
            if(BehaviorsListBox.SelectedIndex != -1)
            {
                BehaviorsListBox.Items.RemoveAt(BehaviorsListBox.SelectedIndex);
            }
        }

        private void AddVariableButton_Click(object sender, RoutedEventArgs e)
        {
            tmp_variables_list.Add((bool)VariableIsStringCheckBox.IsChecked 
                ? 
                new Variable(VariableNameTextBox.Text, VariableValueTextBox.Text) 
                : 
                new Variable(VariableNameTextBox.Text, int.Parse(VariableValueTextBox.Text)));

            UpdateVariablesList();
        }

        private void UpdateVariablesList()
        {
            VariablesListBox.Items.Clear();

            for (int i = 0; i < tmp_variables_list.Count; i++)
            {
                VariablesListBox.Items.Add((

                    (i + 1) + " : " +

                    (tmp_variables_list[i].strv != null
                    ?
                    "(type) string "
                    :
                    "(type) int ") + "" + " : " 
                    + 
                    tmp_variables_list[i].name + " : " 
                    + 
                    (tmp_variables_list[i].strv != null 
                    ? 
                    tmp_variables_list[i].strv 
                    : 
                    tmp_variables_list[i].intv.ToString())));
            }
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle tmpRect = (Rectangle)sender;
            DragDrop.DoDragDrop(tmpRect, tmpRect.Height, DragDropEffects.None);
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
}