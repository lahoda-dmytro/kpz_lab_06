   M S B A M L   `   `   `  ����  	SudokuWPF%    SudokuWPF.View.SavedGamesWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$!  Збережені ігри��5   	   $	��450��6    $	��600��6-    � WindowStartupLocation$ CenterScreen=�5   	   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5	      $
��Auto �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ SavedGamesGrid��5         AutoGenerateColumns .     
IsReadOnly .    SelectionMode$ Single=�5      $��0��5         Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$# Дата збереження��6%   	   Width5  /System.Windows.Controls.DataGridLengthConverter$	 * 6}   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 6>   �� *
SaveDate+ �� StringFormat$ dd.MM.yyyy HH:mm:ss��6}     5      $' !Рівень складності��6%   $	 * 6_   
 6@   �� *
Difficulty+6_   5      �� 5   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   $��0,10,0,0q�6W   .+�5      �� -   5         ��Name$ 
LoadButton��5      $��Завантажити��6)   $	��100��6?   $��0,0,10,0q�6K   6]   �� 5      -   5        $ DeleteButton��5      $��Видалити��6+   $	��100��6>   $��0,0,10,0q�6J   6\   �� 5      -   5        $ CloseButton��5      $��Закрити��6*   $	��100��6<   6H   5      5      5      