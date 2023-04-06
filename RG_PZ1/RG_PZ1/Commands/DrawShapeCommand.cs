using RG_PZ1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace RG_PZ1.Commands
{
    public class DrawShapeCommand : IDrawCommand
    {
        private Canvas _canvas;
        private CanvasElement _canvasElement;
        private Grid _grid;
        private Shape _shape;
        private TextBlock _text;
        private double _setLeft;
        private double _setTop;

        public DrawShapeCommand(Canvas canvas, Shape shape, TextBlock text = null, double setLeft = 0, double setTop = 0)
        {
            _canvasElement = new CanvasElement();
            _canvas = canvas;
            _shape = shape;
            _text = text;
            _setLeft = setLeft;
            _setTop = setTop;
        }

        public void Execute()
        {
            _grid = new Grid();

            Canvas.SetLeft(_grid, _setLeft);
            Canvas.SetTop(_grid, _setTop);

            _grid.Children.Add(_shape);
            if (_text != null)
            {
                _grid.Children.Add(_text);
            }
            _canvasElement.Element = _grid;
            _canvasElement.CanvasChildIndex = _canvas.Children.Add(_grid);
        }

        public void Redo()
        {
            _grid = new Grid();

            Canvas.SetLeft(_grid, _setLeft);
            Canvas.SetTop(_grid, _setTop);

            _grid.Children.Add(_shape);
            if (_text != null)
            {
                _grid.Children.Add(_text);
            }
            _canvasElement.Element = _grid;
            _canvasElement.CanvasChildIndex = _canvas.Children.Add(_grid);
        }

        public void Undo()
        {
            _grid.Children.Remove(_text);
            _grid.Children.Remove(_shape);
            _canvas.Children.Remove(_grid);
        }
    }
}
