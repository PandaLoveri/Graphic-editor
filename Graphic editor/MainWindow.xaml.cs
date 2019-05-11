using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;


namespace Graphic_editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            cnvPaint.Background = Brushes.White;
        }

        /// <summary>
        /// Команды для открытия, сохранения и закрытия файла
        /// </summary>
        private void NewBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {                  
            if (MessageBox.Show(this, "Do you want to save this file?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                SaveBinding_OnExecuted(sender, e);
            Clear();
            cnvPaint.Background = Brushes.White;
        }

        private void OpenBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ImageBrush brush = new ImageBrush();
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.AddExtension = true;
            openDialog.CheckFileExists = true;
            openDialog.DefaultExt = "png";
            openDialog.Filter = "Image files|*.png;*.jpeg;*.ico|All files (*.*)|*.*";
            double imageWidth=0, imageHeight=0;
            if (openDialog.ShowDialog() == true && openDialog.SafeFileName != "")
            {
                Clear();
                brush.ImageSource = new BitmapImage(new Uri(openDialog.FileName));
                imageWidth = brush.ImageSource.Width;
                imageHeight = brush.ImageSource.Height;
                cnvPaint.Width = imageWidth;
                cnvPaint.Height = imageHeight;               
                brush.Stretch = Stretch.Uniform;
                cnvPaint.Background = brush;
            }             
        }

        private void SaveBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {           
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|All(*.*)|*",
                FileName = "Nameless",
                DefaultExt = "png",
            };
            int cw = (int)cnvPaint.Width;
            int ch = (int)cnvPaint.Height;
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(cw, ch, 96d, 96d, PixelFormats.Pbgra32);
            cnvPaint.Measure(new Size(cw, ch));
            cnvPaint.Arrange(new Rect(new Size(cw, ch)));
            renderBitmap.Render(cnvPaint);
            InvalidateVisual();
            if (saveFileDialog.ShowDialog() == true)
            {
                var extension = System.IO.Path.GetExtension(saveFileDialog.FileName);
                using (FileStream file = File.Create(saveFileDialog.FileName))
                {
                    BitmapEncoder encoder = null;
                    switch (extension.ToLower())
                    {
                        case ".jpg":
                            encoder = new JpegBitmapEncoder();
                            break;
                        case ".png":
                            encoder = new PngBitmapEncoder();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(extension);
                    }
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                    encoder.Save(file);
                }
            }
        }

        private void CloseBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
           Close();
        }

        //Метод для очистки холста
        private void Clear()
        {
            cnvPaint.Children.Clear();
            undo_redo.undoCommands.Clear();
            undo_redo.redoCommands.Clear();
            btnRedo.IsEnabled = false;
            btnUndo.IsEnabled = false;
        }
        //------------------------------------------------------------------------------------------


        public ToolType currentTool { get; set; } = ToolType.Pencil;// Текущий выбранный инструмент
        public bool onCanvas = false;                               // флаг о рисовании именно на холсте     
        Point startPoint;                                           // пара координат стартовой точки относительно canvas 
        Shape currentShape = null;
        MouseButtonState previousMouseEvent = new MouseButtonState();//предыдущее состоянии мыши        
        Brush currentBrush = Brushes.Black;
        int currentBrushThickness = 1;
        PathFigure currentFigure;                                    // текущая траектория мыши на холсте, созданная при инструментах карандаш, либо кисть, либо ластик
        System.Windows.Shapes.Path currentPath = null;
        UndoRedo undo_redo = new UndoRedo();
        //---------------------------------------------------------------------------------

        /// <summary>
        ///Обработчики событий выбора инструмента рисования
        /// </summary> 

        //Обработчик события клика на карандаш
        private void BtnPencil_Click(object sender, RoutedEventArgs e)
        {
            currentTool = ToolType.Pencil;
            spThickness.IsEnabled = false;
        }

        //Обработчик события клика на кисточку
        private void BtnBrush_Click(object sender, RoutedEventArgs e)
        {
            currentTool = ToolType.Brush;
            spThickness.IsEnabled = true;
        }
        
        //Обработчик события клика на пипетку
        private void BtnPipette_Click(object sender, RoutedEventArgs e)
        {
            currentTool = ToolType.Pipette;
            spThickness.IsEnabled = false;
        }
        
        //Обработчик события клика на ластик
        private void BtnEraser_Click(object sender, RoutedEventArgs e)
        {
            currentTool = ToolType.Eraser;
            spThickness.IsEnabled = true;
        }
        
        //Обработчики событий кликов на кнопки фигур(линия, эллипс, прямоугольник)

        private void BtnLine_Click(object sender, RoutedEventArgs e)
        {
            currentTool = ToolType.Line;
            spThickness.IsEnabled = true;
        }       

        private void BtnRectangle_Click(object sender, RoutedEventArgs e)
        {
            currentTool = ToolType.Rectangle;
            spThickness.IsEnabled = true;
        }
        
        private void BtnElipse_Click(object sender, RoutedEventArgs e)
        {
            currentTool = ToolType.Ellipse;
            spThickness.IsEnabled = true;
        }
        //------------------------------------------------------------------------------------------

        /// <summary>
        /// Обработчики событий клика на холст(cnvPaint) и движения по нему мышкой
        /// </summary>

        private void CnvPaint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (previousMouseEvent != MouseButtonState.Pressed)
                onCanvas = true;
            startPoint = e.GetPosition(cnvPaint);
            if (currentTool == ToolType.Pipette)
                GetColor((int)e.GetPosition(this).X, (int)e.GetPosition(this).Y);
            if (currentTool == ToolType.Pencil || currentTool == ToolType.Brush || currentTool == ToolType.Eraser)
                StartDraw();
            else if (e.ButtonState == MouseButtonState.Pressed)           
                currentShape = new Line();                       
        }

        private void CnvPaint_MouseMove(object sender, MouseEventArgs e)
        {
            if (onCanvas && OnCanvas(e))
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    switch (currentTool)
                    {
                        case ToolType.Line:
                            cnvPaint.Children.Remove(currentShape);
                            DrawLine(e);
                            break;
                        case ToolType.Rectangle:
                            cnvPaint.Children.Remove(currentShape);
                            DrawRectangle(e);
                            break;
                        case ToolType.Ellipse:
                            cnvPaint.Children.Remove(currentShape);
                            DrawEllipse(e);
                            break;
                        case ToolType.Pencil:
                        case ToolType.Brush:
                        case ToolType.Eraser:
                            cnvPaint.Children.Remove(currentPath);
                            AddDraw(e);
                            break;                       
                    }
                }
                else if (e.LeftButton == MouseButtonState.Released && previousMouseEvent == MouseButtonState.Pressed)
                {
                    if (currentTool == ToolType.Pencil || currentTool == ToolType.Brush || currentTool == ToolType.Eraser)
                    {
                        DrawWithPencilCommand command = new DrawWithPencilCommand(currentPath, cnvPaint);
                        undo_redo.AddComand(command);
                        currentFigure = null;
                        currentPath = null;
                    }
                    else
                    {
                        DrawCommand command = new DrawCommand(currentShape, cnvPaint);
                        undo_redo.AddComand(command);
                    }
                    btnUndo.IsEnabled = true;
                    onCanvas = false;
                }
                previousMouseEvent = e.LeftButton;               
            }            
        }
        //---------------------------------------------------------------------------------
        
        /// <summary>
        ///Метод, определяющий принадлежит ли точка холсту
        /// </summary>
        private bool OnCanvas(MouseEventArgs e)
        {
            if (e.GetPosition(cnvPaint).Y-currentBrushThickness>0)
                return true;
            return false;
        }
        //---------------------------------------------------------------------------------

        /// <summary>
        /// Методы для рисования линии, прямоугольника и эллипса
        /// </summary>
        private void DrawLine(MouseEventArgs e)
        {
            Line line = new Line()
            {
                Stroke = currentBrush,
                StrokeThickness = currentBrushThickness,
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = e.GetPosition(cnvPaint).X,
                Y2 = e.GetPosition(cnvPaint).Y
            };
            cnvPaint.Children.Add(line);
            currentShape = line;
        }

        private void DrawRectangle(MouseEventArgs e)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = Math.Abs(startPoint.X - e.GetPosition(cnvPaint).X);
            rectangle.Height = Math.Abs(startPoint.Y - e.GetPosition(cnvPaint).Y);          
            rectangle.Stroke = currentBrush;
            rectangle.StrokeThickness = currentBrushThickness;

            if (cbFillShape.IsChecked == true)
            {
                rectangle.Fill = currentBrush;
            }

            if (startPoint.X - e.GetPosition(cnvPaint).X > 0)
            {
                Canvas.SetLeft(rectangle, startPoint.X - rectangle.Width);
            }
            else
            {
                Canvas.SetLeft(rectangle, startPoint.X);
            }

            if (startPoint.Y - e.GetPosition(cnvPaint).Y > 0)
            {
                Canvas.SetTop(rectangle, startPoint.Y - rectangle.Height);
            }
            else
            {
                Canvas.SetTop(rectangle, startPoint.Y);
            }

            cnvPaint.Children.Add(rectangle);
            currentShape = rectangle;
        }

        private void DrawEllipse(MouseEventArgs e)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = Math.Abs(startPoint.X - e.GetPosition(cnvPaint).X);
            ellipse.Height = Math.Abs(startPoint.Y - e.GetPosition(cnvPaint).Y);
            ellipse.Stroke = currentBrush;
            ellipse.StrokeThickness = currentBrushThickness;

            if (cbFillShape.IsChecked == true)
            {
                ellipse.Fill = currentBrush;
            }

            if (startPoint.X - e.GetPosition(cnvPaint).X > 0)
            {
                Canvas.SetLeft(ellipse, startPoint.X - ellipse.Width);
            }
            else
            {
                Canvas.SetLeft(ellipse, startPoint.X);
            }

            if (startPoint.Y - e.GetPosition(cnvPaint).Y > 0)
            {
                Canvas.SetTop(ellipse, startPoint.Y - ellipse.Height);
            }
            else
            {
                Canvas.SetTop(ellipse, startPoint.Y);
            }
            cnvPaint.Children.Add(ellipse);
            currentShape = ellipse;
        }
        //--------------------------------------------------------------------------------------  

        /// <summary>
        /// Обработчики событий кликов на кнопки назад и вперед
        /// </summary>
        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            undo_redo.Undo(1);
            btnRedo.IsEnabled = true;
            if (undo_redo.undoCommands.Count == 0)
                btnUndo.IsEnabled = false;
        }

        private void BtnRedo_Click(object sender, RoutedEventArgs e)
        {
            undo_redo.Redo(1);
            btnUndo.IsEnabled = true;
            if (undo_redo.redoCommands.Count == 0)
                btnRedo.IsEnabled = false;
        }
        //--------------------------------------------------------------------------------------  

        /// <summary>
        /// Обработчик события изменения цвета 
        /// </summary>    
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPicker.SelectedColor != null)
            {
                currentBrush = new SolidColorBrush((Color)colorPicker.SelectedColor);
            }
        }
        //--------------------------------------------------------------------------------------  

        /// <summary>
        ///Обработчик события изменения толщины кисти 
        /// </summary> 
        private void SlBrushThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            currentBrushThickness = (int)e.NewValue;
        }
        //--------------------------------------------------------------------------------------  

        /// <summary>
        ///Методы, реализующие логику работы карандаша, кисти, ластика
        /// </summary>
        private void StartDraw()
        {
            currentFigure = new PathFigure() { StartPoint = startPoint };
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path()
            {
                Stroke = currentBrush,
                StrokeThickness = 0.5,                
                Data = new PathGeometry() { Figures = { currentFigure } }
            };
            if (currentTool == ToolType.Eraser)
                path.Stroke =Brushes.White;
            if (currentTool == ToolType.Brush || currentTool == ToolType.Eraser)
                path.StrokeThickness = currentBrushThickness;            
            cnvPaint.Children.Add(path);
            currentPath = path;            
        }

        private void AddDraw(MouseEventArgs e)
        {
            currentFigure.Segments.Add(new LineSegment(e.GetPosition(cnvPaint), isStroked: true));
            currentPath.Data = new PathGeometry() { Figures = { currentFigure } };
            cnvPaint.Children.Add(currentPath);
        }
        //--------------------------------------------------------------------------------------        


        /// <summary>
        ///Получение цвета пикселя изображения
        /// </summary>
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        
        public Color GetPixelColor(IntPtr hwnd, int x, int y)
        {
            IntPtr hdc = GetDC(hwnd);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(hwnd, hdc);
            Color color = Color.FromRgb((byte)(pixel & 0x000000FF), (byte)((pixel & 0x0000FF00) >> 8), (byte)((pixel & 0x00FF0000) >> 16));
            return color;
        }

        public void GetColor(int x, int y)
        {
            IntPtr hwnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            var pixel = GetPixelColor(hwnd, x, y);
            currentBrush = new SolidColorBrush(pixel);
            colorPicker.SelectedColor = pixel;
        }        
        //--------------------------------------------------------------------------------------  

        /// <summary>
        ///Обработчик события нажатия на вкладку окна About
        /// </summary>
        private void MAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Simple graphic editor" + Environment.NewLine + "Program developer: Yakupova Ilsiya Faizovna" +
                            Environment.NewLine + "Project manager: Polezhaev Petr Nikolaevich", "Information about product", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        //--------------------------------------------------------------------------------------  

        /// <summary>
        ///Обработчик события выбора элемента меню "Properties" из вкладки "Edit"
        /// </summary>
        private void MProperties_Click(object sender, RoutedEventArgs e)
        {            
            ImageProperty propertyWindow = new ImageProperty(cnvPaint.Width, cnvPaint.Height) /*{ FieldWidth = cnvPaint.Width, FieldHeight = cnvPaint.Height }*/;
            propertyWindow.Owner = this;
            propertyWindow.Show();    
        }
        //--------------------------------------------------------------------------------------   


        /// <summary>
        ///Закрытие приложения (сообщение)
        /// </summary> 
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                e.Cancel = false;
            else
                e.Cancel = true;
        }          
    }
}
