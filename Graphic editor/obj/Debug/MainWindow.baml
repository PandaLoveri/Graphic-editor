   M S B A M L   `   `   `  ����I  EGraphic editor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Graphic_editorGraphic_editor  5   	       Graphic_editor.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   X TXceed.Wpf.Toolkit, Version=3.4.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4<colorPalette)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   (localclr-namespace:Graphic_editor  5   	   -   5          � Title$  Graphic editor��5	   	   $	��550��6    $
��1015��6-    � Icon$ Icons/icon.ico��6:    � WindowStartupLocation$ CenterScreen=�6P    J� CommandBindings 5
      )  #System.Windows.Input.CommandBinding  5   
   -   5        
NewBinding��   Command$	 New��5   -   6;     5   
   -   5        OpenBinding��$
 Open��5   .   6=     5   
   -   5        SaveBinding��$
 Save��5   .   6=     5   
   -   5        CloseBinding��$ Close��5   /   6?   .��5      ]� ��. .+�5   
   o� $	��Top=�6   .@�5      m� $
��File��6   .@�5      m� $	��New��6    m� Command$	 New��6(    m� Icon 5      �� 5      $��Icons/new.png��6    5      m� 5      $��Open...��6   $
 Open��6,    5      �� 5      $��Icons/open.ico��6    5      m� 5      $��Save...��6   $
 Save��6,    5      �� 5       $��Icons/save.ico��6    5"      �� 5#      m� 5$      $
��Exit��6   $ Close��6)    m� InputGestureText$ Alt+F4��69    5%      �� 5&      $��Icons/exit.png��6    5(      5)      m� 5*      $
��Edit��6   .@�5+      m� -   5        $
��Undo��5+       5,      �� 5-      $��Icons/undo.ico��6    5/      m� 50      -   5        $
��Redo��50       51      �� 52      $��Icons/redo.ico��6    54      �� 55      m� 56      -   5         m�Name$ mProperties��56      $��
Properties��60    57      �� 58      $��Icons/properties.ico��6    5:      5;      m� 5<      -	   5        $ mAbout��5<      $��About��6'   66   5=      `� 5?   
   $	��Top=�6   .�5@      c� .@�5A      �� $	��New��6   .��5B      �� $��26��6   $��Icons/new.png��6'   5C      �� 5D      $
��Open��6   .��5E      �� $��44��6   $��Icons/open.ico��6'   5F      �� 5G      $
��Save��6   .��5H      �� $��36��6   $��Icons/save.ico��6'   5I      5J      c� 5K      .@�5L      �� -
   5        	 ��Name$	 btnUndo��5L      {�.  .��5M      �� $��30��6   $��Icons/undo.ico��6'   5N      �� 5O      -   5        $	 btnRedo��5O      {�.  .��5P      �� $��30��6   $��Icons/redo.ico��6'   5Q      5R      c� 5S      .@�5T      �� -   5        $	 	btnPencil��5T      .��5U      �� $��30��6   $��Icons/pencil.ico��6'   5V      �� 5W      -   5        $	 btnBrush��5W      .��5X      �� $��30��6   $��Icons/brush.ico��6'   5Y      �� 5Z      -   5        $	 
btnPipette��5Z      .��5[      �� $��30��6   $��Icons/pipette.ico��6'   5\      �� 5]      -   5        $	 	btnEraser��5]      .��5^      �� $��30��6   $��Icons/eraser.ico��6'   5_      5`      c� 5a      .@�5b      �� -   5        $��btnLine��5b      .��5c      �� $��Icons/line.ico��6   $��32��64   5d      �� 5e      -   5        $��btnRectangle��5e      .��5f      �� $��Icons/rectangle.ico��6   $��32��69   5g      �� 5h      -   5        $��	btnElipse��5h      .��5i      �� $��Icons/ellipse.ico��6   $��32��67   5j      5k      c� 5l      .@�5m      �� 
 �� Orientation$
 
Horizontal=�6   $��Center=�66   $��Center=�6Q   .+�5n      �� -   5         ��Name$ cbFillShape��5n      $��Center=�64   $��Center=�6X   �� 5o      .��6   
Fill shape6(   5p      5q      c� 5r      .@�5s      #  Xceed.Wpf.Toolkit.ColorPicker  -   5          Name$ colorPicker��5s   +   $��80��6@   $��25��6K      SelectedColor$ BlackX�6W   6m   5t      c� 5u      .@�5v      �� -   5         ��Name$ spThickness��5v      {�.  .+�5w      �� $��Center=�6   $��Center=�6>   .��6b   	Thickness6m   �� 5x      $
 
Horizontal=�6!   .+�5y      �� -   5         ��Name$ slBrushThickness��5y   !   $	��200��6;    � Minimum$ 1Q�6G    � Maximum$ 15Q�6S    �� TickPlacement$
 Both=�6`    �� TickFrequency$ 1Q�6u    �� IsSnapToTickEnabled . 6�   �� 5z      -   5        $��tBrushThickness��5z   "   $��20��6:   $��20��6E   {�.  $��Center=�6�   $��Center=�6�   ��6Q   ��  �� ElementName$ slBrushThickness�� �� Path$ Value�6�   5{      5|      5}      5~      �� 5   
   -   5         ��Name$ 	scrViewer��5      $
��Auto=�6*   $
��Auto=�6O   .��5�      �� -   5         ��Name$ cnvPaint��5�      $
��3000��6y   $
��3000��6�   5�      5�      5�      5�      