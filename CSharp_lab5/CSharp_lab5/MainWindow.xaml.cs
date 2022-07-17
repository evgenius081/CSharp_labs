using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Linq;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace PT_lab11
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public Newton newtonSymbol;
        public Compresser compresser;
        private int highestPercentageReached = 0;
        
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void NewtonSymbolTasks(object sender, RoutedEventArgs e)
        {
            int k, n;
            if (!Int32.TryParse(textBoxN.Text, out n) || !Int32.TryParse(textBoxK.Text, out k))
            {
                SetErrorLabel("Enter K and N as integers please");
                return;
            }
            newtonSymbol = new Newton(n, k);
            double result = newtonSymbol.CalculateWithTasks();
            switch (result)
            {
                case -1:
                    SetErrorLabel("N and K must be greater than 0");
                    break;
                case -2:
                    SetErrorLabel("K must be lesser than N");
                    break;
                default:
                    textBoxTasks.Text = result.ToString();
                    SetErrorLabel("");
                    break;
            }
            
        }
        
        private void NewtonSymbolDelegates(object sender, RoutedEventArgs e)
        {
            int k, n;
            if (!Int32.TryParse(textBoxN.Text, out n) || !Int32.TryParse(textBoxK.Text, out k))
            {
                SetErrorLabel("Enter K and N as integers please");
                return;
            }
            newtonSymbol = new Newton(n, k);
            double result = newtonSymbol.CalculateWithDelegates();
            switch (result)
            {
                case -1:
                    SetErrorLabel("N and K must be greater than 0");
                    break;
                case -2:
                    SetErrorLabel("K must be lesser than N");
                    break;
                default:
                    textBoxDelegates.Text = result.ToString();
                    SetErrorLabel("");
                    break;
            }
            
        }
        
        private async void NewtonSymbolAsyncAwait(object sender, RoutedEventArgs e)
        {
            int k, n;
            if (!Int32.TryParse(textBoxN.Text, out n) || !Int32.TryParse(textBoxK.Text, out k))
            {
                SetErrorLabel("Enter K and N as integers please");
                return;
            }
            newtonSymbol = new Newton(n, k);
            double result = await newtonSymbol.CalculateWithAsyncAwait();
            switch (result)
            {
                case -1:
                    SetErrorLabel("N and K must be greater than 0");
                    break;
                case -2:
                    SetErrorLabel("K must be lesser than N");
                    break;
                default:
                    textBoxAsyncAwait.Text = result.ToString(CultureInfo.InvariantCulture);
                    SetErrorLabel("");
                    break;
            }
        }

        public void Fibonacci(object sender, RoutedEventArgs e)
        {
            int i;
            if(!Int32.TryParse(textBoxI.Text, out i))
            {
                SetErrorLabel("Enter integer i please");
            }
            
            if (i <= 0)
            {
                SetErrorLabel("i must be greater than 0");
                return;
            }
            BackgroundWorker fibonacciWorker = new BackgroundWorker();
            fibonacciWorker.DoWork += fibonacciWorker_DoWork;
            fibonacciWorker.RunWorkerCompleted += fibonacciWorker_RunWorkerCompleted;
            fibonacciWorker.ProgressChanged += fibonacciWorker_ProgressChanged;
            progressBar.Value = 0;
            fibonacciWorker.RunWorkerAsync(i);
        }
        
        private void fibonacciWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker != null)
            {
                worker.WorkerReportsProgress = true;
                e.Result = ComputeFibonacci((int) e.Argument, worker, e);
            }
        }

        private void fibonacciWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBoxFibonacci.Text = e.Result.ToString();
        }

        private void fibonacciWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private UInt64 ComputeFibonacci(int n, BackgroundWorker worker, DoWorkEventArgs e)
        {
            if(n <= 0)
            {
                SetErrorLabel("i must be greater than 0");
                return 0;
            }
            UInt64 result = 0;

            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                List<UInt64> listOfFibonacciElements = new List<UInt64>();
                
                for(int i = 1; i <= n; i++)
                {
                    if (i <= 2)
                    {
                        listOfFibonacciElements.Add(1);
                    }
                    else
                    {
                        var a = listOfFibonacciElements.Last();
                        listOfFibonacciElements.Remove(a);
                        var b = listOfFibonacciElements.Last();
                        listOfFibonacciElements.Add(a);
                        listOfFibonacciElements.Add(a + b);
                    }
                    int percentComplete = (int)((float)i / n * 100);
                    if (percentComplete > highestPercentageReached)
                    {
                        highestPercentageReached = percentComplete;
                        worker.ReportProgress(percentComplete);
                        Thread.Sleep(20);
                    }
                }
                result = listOfFibonacciElements.Last();
            }
            

            return result;
        }

        public void Compress(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                compresser = new Compresser();
                Compresser.Compress(dialog.SelectedPath);
            }
        }
        
        public void Decompress(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                compresser = new Compresser();
                compresser.Decompress(dialog.SelectedPath);
            }
        }

        public void Resolve(object sender, RoutedEventArgs e)
        {
            var result = Resolver.Convert();
            textBoxOutput.Text = "";
            foreach (var domain in result)
            {
                textBoxOutput.Text += $"{domain.Item1} -> {domain.Item2}\n";
            }
        }

        private void SetErrorLabel(string error)
        {
            labelError.Content = error;
        }
    }
}