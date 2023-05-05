using Lunar.Core;
using SkiaSharp;
using System.Collections.ObjectModel;
namespace Lunar.Controls
{
    public class MultiChildContainer : Container
    {
        public ObservableCollection<Control> Children { get; set; }
        
        public delegate void OnChildChangedDelegate(Control child);
        public event OnChildChangedDelegate OnChildAdded;
        public event OnChildChangedDelegate OnChildRemoved;
        
        public void AddChild(Control control)
        {
            OnChildAdded?.Invoke(control);
            Children.Add(control);
        }

        public void RemoveChild(Control control)
        {
            OnChildRemoved?.Invoke(control);
            Children.Add(control);
        }

        public void RemoveChild(int index)
        {
            OnChildRemoved?.Invoke(Children[index]);
            Children.RemoveAt(index);
        }

        public override void UpdateChildren(double dt)
        {
            base.UpdateChildren(dt);
            for(var i = 0; i < Children.Count; i++)
            {
                Children[i].OnUpdate(dt);
            }
        }
        public override void RenderChildren(SKCanvas canvas)
        {
            base.RenderChildren(canvas);
            for(var i = 0; i < Children.Count; i++)
            {
                Children[i].OnRender(canvas);
            }
        }
    }
}
