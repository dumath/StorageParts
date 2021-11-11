using System;
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

        //TODO:parts в viewModel. Если будет подключаться облако.
        private List<Part> parts; //Список запчастей. Синхронизация с listBoxItems.

        List<string> strings; //Коллекция, для инициализации класса Part.//Тестовое.

        List<ListBoxItem> listBoxItems; //Коллекция выделения строки.

        #region Constructors
        public MainPage()
        {
            this.InitializeComponent();
            this.activeWindow = storageWindow; //Инициализируем стартовуя сетку.
            this.parts = new List<Part>(); //Убираем null, для дальнейшей синхронизации.
            this.listBoxItems = new List<ListBoxItem>(); //Убираем null, для дальнейшей синхронизации выбранного объекта.
        }
        #endregion

        #region AppBarButtons 
        /// <summary>
        /// Метод, выводящий форму сетки добавления объекта.
        /// </summary>
        /// <param name="sender">Не используется.</param>
        /// <param name="e">Не используется.</param>
        private void Click_Add(object sender, RoutedEventArgs e)
        {
            //Меняем Grid
            this.activeWindow.Visibility = Visibility.Collapsed;
            this.activeWindow = addItemPage;
            this.activeWindow.Visibility = Visibility.Visible;
            //Очищаем форму.
            brandField.Text = String.Empty;
            nameField.Text = String.Empty;
            onNumField.Text = String.Empty;
            anNumField.Text = String.Empty;
            countField.Text = String.Empty;
            buyField.Text = String.Empty;
            sellField.Text = String.Empty;
            fcField.Text = String.Empty;
            scField.Text = String.Empty;
        }

        /// <summary>
        /// Метод изменения данных объекта, выделенного в сетке StorageTable(PS:TWO)
        /// </summary>
        /// <param name="sender">Не используется.</param>
        /// <param name="e">Не используется.</param>
        private void Click_Edit(object sender, RoutedEventArgs e)
        {
            if (listBoxItems.Count != 0)
            {
                activeWindow.Visibility = Visibility.Collapsed;
                activeWindow = addItemPage;
                activeWindow.Visibility = Visibility.Visible;
                buyField.IsEnabled = false;
                int id = Int32.Parse(listBoxItems[0].Content.ToString());
                Part p = parts.Find(x => x.ID == id);
                brandField.Text = p.Brand;
                nameField.Text = p.Name;
                onNumField.Text = p.OriginalNumber;
                anNumField.Text = p.AnalogNumber;
                countField.Text = p.Count.ToString();
                buyField.Text = p.BuyPrice.ToString();
                sellField.Text = p.SellPrice.ToString();
                fcField.Text = p.FirstComment;
                scField.Text = p.SecondComment;
                addButton.Visibility = Visibility.Collapsed;
                changeButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Метод, удаляющий объект, выделенный в сетке StorageTable(PS:TWO)
        /// </summary>
        /// <param name="sender">Не используется.</param>
        /// <param name="e">Не используется.</param>
        private void Click_Delete(object sender, RoutedEventArgs e)
        {
            if (listBoxItems.Count != 0)
            {
                //Ищем значение id, для поиска индекса в parts
                int id = int.Parse(listBoxItems[0].Content.ToString());
                Part part = this.parts.Find(x => x.ID == id);
                //Ищем индекс
                int index = numColumn.Children.IndexOf(this.listBoxItems[0]);
                //Очищаем таблицу
                numColumn.Children.Remove(this.listBoxItems[0]);
                brandColumn.Children.Remove(this.listBoxItems[1]);
                nameColumn.Children.Remove(this.listBoxItems[2]);
                originalColumn.Children.Remove(this.listBoxItems[3]);
                analogColumn.Children.Remove(this.listBoxItems[4]);
                countColumn.Children.Remove(this.listBoxItems[5]);
                buyPriceColumn.Children.Remove(this.listBoxItems[6]);
                sellPriceColumn.Children.Remove(this.listBoxItems[7]);
                firstCommentColumn.Children.Remove(this.listBoxItems[8]);
                secondCommentColumn.Children.Remove(this.listBoxItems[9]);
                //Очищаем выделнную строку.
                this.listBoxItems.Clear();
                this.parts.Remove(part);
                //Выключаем кнопки. Выделение снято.
                editPartButton.IsEnabled = false;
                deletePartButton.IsEnabled = false;
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
                catch (Exception ex)
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

        /// <summary>
        /// Метода добавления данных в сетку таблицы.
        /// </summary>
        /// <param name="s">Значение</param>
        /// <param name="stackPanel">Столбец в таблице</param>
        private void putData(string s, StackPanel stackPanel)
        {
            ListBoxItem listBoxItem = new ListBoxItem() { Content = s, Style = (Style)Application.Current.Resources["stackElement"] };
            listBoxItem.Tapped += Tap_To_Table;
            stackPanel.Children.Add(listBoxItem);
        }

        /// <summary>
        /// Метод сохранения объекта из сетки описания свойств запчасти.
        /// </summary>
        /// <param name="sender">Не используется.</param>
        /// <param name="e">Не используется.</param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Считываем данные, введеные пользователем.
                int count = int.Parse(countField.Text);
                decimal bp = decimal.Parse(buyField.Text);
                decimal sp = decimal.Parse(sellField.Text);
                //Создаем объект. Тут статическое поле увеличивается.(PS:Для теста).
                Part p = new Part(bp)
                {
                    Brand = brandField.Text,
                    Name = nameField.Text,
                    OriginalNumber = onNumField.Text,
                    AnalogNumber = anNumField.Text,
                    Count = count,
                    SellPrice = sp,
                    FirstComment = fcField.Text,
                    SecondComment = scField.Text
                };
                //Добавляем в коллекцию для синхронизации.
                parts.Add(p);
                //Заполняем таблицу.
                this.putData(p.ID.ToString(), numColumn);
                this.putData(p.Brand, brandColumn);
                this.putData(p.Name, nameColumn);
                this.putData(p.OriginalNumber, originalColumn);
                this.putData(p.AnalogNumber, analogColumn);
                this.putData(p.Count.ToString(), countColumn);
                this.putData(p.BuyPrice.ToString(), buyPriceColumn);
                this.putData(p.SellPrice.ToString(), sellPriceColumn);
                this.putData(p.FirstComment, firstCommentColumn);
                this.putData(p.SecondComment, secondCommentColumn);

                //Возвращаемся на сетку StorageTable(PS:TWO)
                this.ReturnToMainWindow(null, null);
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.Message, ex.Source);
                msg.ShowAsync();
            }
        }

        /// <summary>
        /// Метод изменения объекта в сетке StorageTable(PS:Two0.
        /// </summary>
        /// <param name="sender">Не исользуется.</param>
        /// <param name="e">Не используется.</param>
        private void Change_CLick(object sender, RoutedEventArgs e)
        {
            int id = Int32.Parse(listBoxItems[0].Content.ToString());
            Part p = this.parts.Find(x => x.ID == id);
            //Обновляем значение в коллекции синхронизации.
            p.Brand = brandField.Text;
            p.Name = nameField.Text;
            p.OriginalNumber = onNumField.Text;
            p.AnalogNumber = anNumField.Text;
            p.Count = Int32.Parse(countField.Text);
            p.SellPrice = decimal.Parse(sellField.Text);
            p.FirstComment = fcField.Text;
            p.SecondComment = scField.Text;
            //Обновляем значение в списке выбранных данных.
            listBoxItems[1].Content = p.Brand;
            listBoxItems[2].Content = p.Name;
            listBoxItems[3].Content = p.OriginalNumber;
            listBoxItems[4].Content = p.AnalogNumber;
            listBoxItems[5].Content = p.Count.ToString();
            listBoxItems[7].Content = p.SellPrice.ToString();
            listBoxItems[8].Content = p.FirstComment;
            listBoxItems[9].Content = p.SecondComment;
            //Меняем кнопки. Кнопка "Сохранить" включена с инициализации.
            changeButton.Visibility = Visibility.Collapsed;
            addButton.Visibility = Visibility.Visible;
            //Поле только для чтения, для добавления. Включено с инициализации.
            buyField.IsEnabled = true;
            this.ReturnToMainWindow(null, null); //Возвращаемся на сетку таблицы.
        }

        /// <summary>
        /// Метод выбора строки в таблице
        /// </summary>
        /// <param name="sender">Элемент, вызвавший событие.</param>
        /// <param name="e">Не используется.</param>
        private void Tap_To_Table(object sender, TappedRoutedEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;
            if (listBoxItem == null)
                return;
            StackPanel stackPanel = listBoxItem.Parent as StackPanel;
            if (stackPanel == null)
                return;
            int i = stackPanel.Children.IndexOf(listBoxItem);
            if (listBoxItems.Count != 0)
            {
                foreach (ListBoxItem l in listBoxItems)
                    l.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);

                listBoxItems.Clear();
            }
            listBoxItems.Add(numColumn.Children[i] as ListBoxItem);
            listBoxItems.Add(brandColumn.Children[i] as ListBoxItem);
            listBoxItems.Add(nameColumn.Children[i] as ListBoxItem);
            listBoxItems.Add(originalColumn.Children[i] as ListBoxItem);
            listBoxItems.Add(analogColumn.Children[i] as ListBoxItem);
            listBoxItems.Add(countColumn.Children[i] as ListBoxItem);
            listBoxItems.Add(buyPriceColumn.Children[i] as ListBoxItem);
            listBoxItems.Add(sellPriceColumn.Children[i] as ListBoxItem);
            listBoxItems.Add(firstCommentColumn.Children[i] as ListBoxItem);
            listBoxItems.Add(secondCommentColumn.Children[i] as ListBoxItem);
            foreach (ListBoxItem l in listBoxItems)
                l.Background = new SolidColorBrush(Windows.UI.Colors.Beige);
            //Включаем кнопки.Таблица не пуста.
            editPartButton.IsEnabled = true;
            deletePartButton.IsEnabled = true;
        }
        #endregion
    }
}
