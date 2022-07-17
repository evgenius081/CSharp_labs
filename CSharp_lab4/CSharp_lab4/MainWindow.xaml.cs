using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace PT_lab10._2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private CarBindingList myCarsBindingList;
        private BindingSource carBindingSource;
        private Dictionary<string, bool> sortingType = new Dictionary<string, bool>();
        
        public List<Car> myCars;
        
        public MainWindow(List<Car> cars)
        {
            myCars = cars;
            InitializeComponent();
            InitComboBox();
            InitSorting();
            myCarsBindingList = new CarBindingList(myCars);
            carBindingSource = new BindingSource();
            UpdateDataGrid();
        }

        private void InitSorting()
        {
            sortingType.Clear();
            sortingType.Add("model", false);
            sortingType.Add("motor", false);
            sortingType.Add("year", false);

        }

        private void ButtonSearch(object sender, RoutedEventArgs e)
        {
            CheckForNewItems();
            myCarsBindingList = new CarBindingList(myCars);
            List<Car> resultListOfCars;
            Int32 tmp;
            if (!searchTextBox.Text.Equals(""))
            {
                //OutputWriter.Write(comboBox.SelectedItem.ToString());
                string property = comboBox.SelectedItem.ToString();
                if (Int32.TryParse(searchTextBox.Text, out tmp))
                {
                    resultListOfCars = myCarsBindingList.FindCars(property, tmp);
                }
                else
                {
                    resultListOfCars = myCarsBindingList.FindCars(property, searchTextBox.Text);
                }

                myCarsBindingList = new CarBindingList(resultListOfCars);
                UpdateDataGrid();
            }
        }

        private void CheckForNewItems()
        {
            foreach(Car item in myCarsBindingList)
            {
                if (!myCars.Contains(item))
                {
                    myCars.Add(item);
                }
            }
        }

        private void ButtonReload(object sender, RoutedEventArgs e)
        {
            myCarsBindingList = new CarBindingList(myCars);
            UpdateDataGrid();
        }
        private void SortColumn(object sender, RoutedEventArgs e)
        {
            var columnHeader = sender as DataGridColumnHeader;
            string columnName = columnHeader.ToString().Split(' ')[1].ToLower();
            bool isAsc = sortingType[columnName];
            InitSorting();
            if (isAsc == true)
            {
                myCarsBindingList.Sort(columnName, ListSortDirection.Descending);
            }
            else
            {
                myCarsBindingList.Sort(columnName, ListSortDirection.Ascending);
            }
            sortingType[columnName] = !isAsc;
            UpdateDataGrid();
        }

        private void ButtonDeleteRow(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow) vis;
                    Car car = (Car) row.Item;
                    myCarsBindingList.Remove(car);
                    myCars.Remove(car);
                    UpdateDataGrid();
                    break;
                }
        }

        private void UpdateDataGrid()
        {

            carBindingSource.DataSource = myCarsBindingList;
            dataGridView1.ItemsSource = carBindingSource;
            
        }

        private void InitComboBox()
        {
            BindingList<string> list = new BindingList<string>();
            list.Add("model");
            list.Add("year");
            list.Add("motor.displacement");
            list.Add("motor.model");
            list.Add("motor.horsePower");
            comboBox.ItemsSource = list;
            comboBox.SelectedIndex = 0;
        }
    }
}