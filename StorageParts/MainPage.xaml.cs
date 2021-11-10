﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AccauntObject;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;


// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace StorageParts
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Grid activeWindow; //Переменная, отслеживающая активное окно.
        private List<Part> parts; //Список запчастей
        List<string> strings; //Коллекция, для инициализации класса Part

        #region Constructors
        public MainPage()
        {
            this.InitializeComponent();
            this.activeWindow = storageWindow;
            this.parts = new List<Part>();
            
        }
        #endregion

        #region AppBarButtons 
        //Добавить новую позицию.
        private void Click_Add(object sender, RoutedEventArgs e)
        {
            //Меняем Grid
            this.activeWindow.Visibility = Visibility.Collapsed;
            this.activeWindow = addItemPage;
            this.activeWindow.Visibility = Visibility.Visible;
            //Очищаем форму.
            brandName.Text = String.Empty;
            nameObjectField.Text = String.Empty;
            onNumField.Text = String.Empty;
            anNumField.Text = String.Empty;
            countObjectField.Text = String.Empty;
            buy.Text = String.Empty;
            sellField.Text = String.Empty;
            fcField.Text = String.Empty;
            scField.Text = String.Empty;
        }

        //Изменить позицию.
        private void Click_Edit(object sender, RoutedEventArgs e)
        {
            if (storageTable.SelectedIndex != -1)
            {
                string s = storageTable.SelectedItem.ToString();
                if (s != null)
                {
                    int p = parts.FindIndex(x => x.Equals(new Part(s)));
                    activeWindow.Visibility = Visibility.Collapsed;
                    activeWindow = addItemPage;
                    activeWindow.Visibility = Visibility.Visible;
                    buy.IsEnabled = false;
                    brandName.Text = parts[p].Brand;
                    nameObjectField.Text = parts[p].Name;
                    onNumField.Text = parts[p].OriginalNumber;
                    anNumField.Text = parts[p].AnalogNumber;
                    countObjectField.Text = parts[p].Count.ToString();
                    buy.Text = parts[p].BuyPrice.ToString();
                    sellField.Text = parts[p].SellPrice.ToString();
                    fcField.Text = parts[p].FirstComment;
                    scField.Text = parts[p].SecondComment;
                    addButton.Visibility = Visibility.Collapsed;
                    changeButton.Visibility = Visibility.Visible;
                }
            }
        }

        //Удалить позицию.
        private void Click_Delete(object sender, RoutedEventArgs e)
        {
            if (storageTable.SelectedIndex != -1)
            {
                string s = storageTable.SelectedItem.ToString();
                int i = this.parts.FindIndex(x => x.Equals(new Part(s)));
                this.parts.RemoveAt(i);
                storageTable.Items.RemoveAt(storageTable.SelectedIndex);
            }
        }
        #endregion

        #region Left Panel
        //Метод, который выводит левую панель кнопок, если наведен ккурсор.
        private void OpenPane(object sender, PointerRoutedEventArgs e)
        {
            fullScreenView.IsPaneOpen = true;
        }

        //Метод, который скрывает левую панель кнопок, если курсор вышел из области действия панели.
        private void ClosePane(object sender, PointerRoutedEventArgs e)
        {
            fullScreenView.IsPaneOpen = false;
        }
        #endregion

        #region Methods Open/Close/Save
        //Создать файл. TODO: Test
        private async void createFile_Click(object sender, RoutedEventArgs e)
        {
            if (storageTable.Items.Count != 0)
            {
                ContentDialog contentDialog = new ContentDialog();
                contentDialog.Title = "Создание нового файла";
                contentDialog.Content = "Все имзенения будут отменены. Сохранить файл?";
                contentDialog.PrimaryButtonText = "Сохранить";
                contentDialog.SecondaryButtonText = "Не сохранять";
                contentDialog.CloseButtonText = "Отмена";
                var result = await contentDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    if (await saveFile())
                    {
                        this.parts.Clear();
                        storageTable.Items.Clear();
                    }
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    this.parts.Clear();
                    storageTable.Items.Clear();
                }
                else
                {
                    return;
                }
                return;
            }
            return;
        }

        //Открыть файл. Полностью готов.
        private async void selectFile_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            fileOpenPicker.ViewMode = PickerViewMode.List;
            fileOpenPicker.FileTypeFilter.Add("*");
            StorageFile storageFile = await fileOpenPicker.PickSingleFileAsync();
            this.strings = new List<string>();
            if (storageFile != null)
            {
                try
                {
                    storageTable.Items.Clear();
                    var fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite);
                    var inputStream = fileStream.GetInputStreamAt(0);
                    using (TextReader reader = new StreamReader(inputStream.AsStreamForRead()))
                    {
                        this.parts.Clear();
                        string s = reader.ReadLine();
                        while (s != null)
                        {
                            this.initializeDB(s);
                            s = reader.ReadLine();
                        }
                    }
                    ((IDisposable)fileStream).Dispose();
                }
                catch(Exception ex)
                {
                    MessageDialog msg = new MessageDialog(ex.Message);
                    msg.ShowAsync();
                }
            }
        }

        //Сохранить как файл.Test
        private async void saveAsFile_Click(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                FileSavePicker fileSavePicker = new FileSavePicker();
                fileSavePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                fileSavePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
                fileSavePicker.SuggestedFileName = "New Document";
                var newFile = await fileSavePicker.PickSaveFileAsync();
                if (newFile != null)
                {
                    List<string> strings = new List<string>();
                    foreach (Part p in parts)
                    {
                        strings.Add(p.ToString());
                    }
                    await FileIO.WriteLinesAsync(newFile, strings);
                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + " Метод Сохранить как");
                msg.ShowAsync();
            }
            finally
            {

            }

        }

        //Сохранить файл.
        //TODO:Добавить сохранение без вывода окна открытого файла
        private async void saveFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileSavePicker fileSavePicker = new FileSavePicker();
                fileSavePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                fileSavePicker.FileTypeChoices.Add("Plain text", new List<string>() { ".txt" });
                fileSavePicker.SuggestedFileName = "New Document";

                var newFile = await fileSavePicker.PickSaveFileAsync();
                if (newFile != null)
                {
                    List<string> strings = new List<string>();
                    foreach (Part p in parts)
                    {
                        strings.Add(p.ToString());
                    }

                    await FileIO.WriteLinesAsync(newFile, strings);

                }
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message);
                msg.ShowAsync();
            }
            finally
            {

            }
        }

        //Дополнительный метод сохранения.TODO: Тест, заменить.
        private async System.Threading.Tasks.Task<bool> saveFile()
        {
            try
            {
                FileSavePicker fileSavePicker = new FileSavePicker();
                fileSavePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                fileSavePicker.FileTypeChoices.Add("Plain text", new List<string>() { ".txt" });
                fileSavePicker.SuggestedFileName = "New Document";
                var newFile = await fileSavePicker.PickSaveFileAsync();
                if (newFile != null)
                {
                    List<string> strings = new List<string>();
                    foreach (Part p in parts)
                    {
                        strings.Add(p.ToString());
                    }
                    await FileIO.WriteLinesAsync(newFile, strings);
                    strings.Clear();
                    
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message + "Внутренний сохранить");
                msg.ShowAsync();
                return false;
            }
        }
        #endregion

        #region Other Methods. Для упрощения кода.
        //Мето возвращения на сетку главного окна.
        private void ReturnToMainWindow(object sender, RoutedEventArgs e)
        {
            this.activeWindow.Visibility = Visibility.Collapsed;
            this.activeWindow = storageWindow;
            this.storageWindow.Visibility = Visibility.Visible;
            changeButton.Visibility = Visibility.Collapsed;
            addButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Метод инициализации коллекции parts из выбранного пользователем файла.
        /// </summary>
        /// <param name="s">Строка файла, инициализирующая один объект класса Part</param>
        private void initializeDB(string s)
        {
            Part p = new Part(s);
            if (parts != null)
            {
                this.parts.Add(p);
                storageTable.Items.Add(p.ToString());
            }
        }

        private void putData(string s, RowDefinition rowDefinition, ColumnDefinition columnDefinition)
        {
            TextBlock textBlock = new TextBlock() { Text = s, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch };
            storageTableTwo.Children.Add(textBlock);
            Grid.SetRow(textBlock, storageTableTwo.RowDefinitions.IndexOf(rowDefinition));
            Grid.SetColumn(textBlock, storageTableTwo.ColumnDefinitions.IndexOf(columnDefinition));
        }

        //Метод добавления объекта в сетку.
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Считываем данные, введеные пользователем.
                int count = int.Parse(countObjectField.Text);
                decimal bp = decimal.Parse(buy.Text);
                decimal sp = decimal.Parse(sellField.Text);
                Part p = new Part(bp)
                {
                    Brand = brandName.Text,
                    Name = nameObjectField.Text,
                    OriginalNumber = onNumField.Text,
                    AnalogNumber = anNumField.Text,
                    Count = count,
                    SellPrice = sp,
                    FirstComment = fcField.Text,
                    SecondComment = scField.Text
                };
                parts.Add(p);

                RowDefinition rowDefinition = new RowDefinition() { Height = new GridLength(20) };
                storageTableTwo.RowDefinitions.Add(rowDefinition);
                this.putData(Part.Number.ToString(), rowDefinition, zeroColumn);
                this.putData(p.Brand, rowDefinition, oneColumn);
                this.putData(p.Name, rowDefinition, twoColumn);
                this.putData(p.OriginalNumber, rowDefinition, threeColumn);
                this.putData(p.AnalogNumber, rowDefinition, fourthColumn);
                this.putData(p.Count.ToString(), rowDefinition, fiveColumn);
                this.putData(p.BuyPrice.ToString(), rowDefinition, sixColumn);
                this.putData(p.SellPrice.ToString(), rowDefinition, sevenColumn);
                this.putData(p.FirstComment, rowDefinition, eigthColumn);
                this.putData(p.SecondComment, rowDefinition, nineColumn);
                //storageTable.Items.Add(p.ToString()); отключаем тестовую разметку.
                this.ReturnToMainWindow(null, null);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message);
                msg.ShowAsync();
            }


        }

        //Метод изменения объекта в сетке.
        private void Change_CLick(object sender, RoutedEventArgs e)
        {
            string s = storageTable.SelectedItem.ToString();
            int iST = storageTable.SelectedIndex;
            storageTable.Items.RemoveAt(iST);
            int i = parts.FindIndex(x => x.Equals(new Part(s)));
            int c = Int32.Parse(countObjectField.Text);
            decimal sp = decimal.Parse(sellField.Text);
            parts[i].Brand = brandName.Text;
            parts[i].Name = nameObjectField.Text;
            parts[i].OriginalNumber = onNumField.Text;
            parts[i].AnalogNumber = anNumField.Text;
            parts[i].Count = c;
            parts[i].SellPrice = sp;
            parts[i].FirstComment = fcField.Text;
            parts[i].SecondComment = scField.Text;
            storageTable.Items.Insert(iST, parts[i].ToString());
            this.ReturnToMainWindow(null, null);
        }
        #endregion



    }

}
