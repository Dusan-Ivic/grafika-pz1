using RG_PZ1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RG_PZ1.Commands
{
    public class AddTextCommand : IDrawCommand
    {
        private Canvas _canvas;
        private CanvasText _canvasText;
        private TextBlock _textBlock;
        private double _setLeft;
        private double _setTop;

        public AddTextCommand(Canvas canvas, TextBlock textBlock, double setLeft, double setTop)
        {
            _canvasText = new CanvasText();
            _canvas = canvas;
            _textBlock = textBlock;
            _setLeft = setLeft;
            _setTop = setTop;
        }

        public void Execute()
        {
            Canvas.SetLeft(_textBlock, _setLeft);
            Canvas.SetTop(_textBlock, _setTop);

            _canvasText.TextBlock = _textBlock;
            _canvasText.CanvasChildIndex = _canvas.Children.Add(_textBlock);
        }

        public void Redo()
        {
            _canvasText.TextBlock = _textBlock;
            _canvasText.CanvasChildIndex = _canvas.Children.Add(_textBlock);
        }

        public void Undo()
        {
            _canvas.Children.Remove(_textBlock);
        }
    }
}
