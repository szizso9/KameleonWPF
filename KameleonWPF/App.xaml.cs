using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Kameleon2.Model;
using Kameleon2.Persistence;
using Kameleon2.ViewModel;
using Microsoft.Win32;
using Kameleon2.View;

namespace Kameleon2
{

    public partial class App : Application
    {
        private KameleonGameModel _model = null!;
        private KameleonViewModel _viewModel = null!;
        private MainWindow _view = null!;
        
        

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object? sender, StartupEventArgs e)
        {
            
            _model = new KameleonGameModel(new KameleonFileDataAccess());
            _model.GameOver += new EventHandler<KameleonEventArgs>(Model_GameOver);
            _model.fNewGame();

            
            _viewModel = new KameleonViewModel(_model);
            _viewModel.NewGame += new EventHandler(ViewModel_NewGame);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);

            
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing); 
            _view.Show();

            


        }

        

        private void View_Closing(object? sender, CancelEventArgs e)
        {

            if (MessageBox.Show("Biztos, hogy ki akar lépni?", "Kaméleon", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; 
            }
        }

        private void ViewModel_NewGame(object? sender, EventArgs e)
        {
            _model.fNewGame();
           

        }

        private async void ViewModel_LoadGame(object? sender, System.EventArgs e)
        {
           

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog(); 
                openFileDialog.Title = "Kaméleon tábla betöltése";
                openFileDialog.Filter = "Kaméleon tábla|*.stl";
                if (openFileDialog.ShowDialog() == true)
                {
                   
                    await _model.LoadGameAsync(openFileDialog.FileName);

                  
                }
            }
            catch (KameleonDataException)
            {
                MessageBox.Show("A fájl betöltése sikertelen!", "Kaméleon", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
        }

        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog(); 
                saveFileDialog.Title = "Sudoku tábla betöltése";
                saveFileDialog.Filter = "Sudoku tábla|*.stl";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        
                        await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (KameleonDataException)
                    {
                        MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("A fájl mentése sikertelen!", "Kaméleon", MessageBoxButton.OK, MessageBoxImage.Error);
            }

          
        }

        private void ViewModel_ExitGame(object? sender, System.EventArgs e)
        {
            _view.Close(); 
        }

        private void Model_GameOver(object? sender, KameleonEventArgs e)
        {
                MessageBox.Show("A győztes: "+e.Player,
                                "Kaméleon",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
        }
    }
}
