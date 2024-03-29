﻿using Lunar.Core;
using SkiaSharp;
using System.Collections.ObjectModel;
using Lunar.Native;
namespace Lunar.Controls
{
    public class MultiChildContainer : Container
    {
        public ObservableCollection<Control> Children { get; set; } = new();
        
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

        public override void OnUpdate(double dt)
        {
            base.OnUpdate(dt);
            for(var i = 0; i < Children.Count; i++)
            {
                Children[i].OnUpdate(dt);
            }
        }
        public override void OnRender(SKCanvas canvas)
        {
            base.OnRender(canvas);
            for(var i = 0; i < Children.Count; i++)
            {
                Children[i].OnRender(canvas);
            }
        }
        
        public override void OnResized(Vector2 newSize)
        {
            base.OnResized(newSize);
            for(var i = 0; i < Children.Count; i++)
            {
                Children[i].OnResized(Children[i].Size);
            }
        }
        public override void ClearChildren()
        {
            Children.Clear();
        }
        public override void ApplyStyles()
        {
            base.ApplyStyles();
            foreach (var child in Children)
            {
                child.ApplyStyles();
            }
        }
        public override void OnMouseMove(ref MouseEvent e, Vector2 position)
        {
            foreach (var child in Children)
            {
                child.OnMouseMove(ref e, position);
            }
            base.OnMouseMove(ref e, position);
        }

        public override void OnMouseButton(ref MouseEvent ev, MouseButton button, bool pressed, Vector2 position)
        {
            foreach (var child in Children)
            {
                child.OnMouseButton(ref ev, button, pressed, position);
            }
            base.OnMouseButton(ref ev, button, pressed, position);
        }
        public MultiChildContainer(Window window) : base(window)
        {
        }
    }
}
