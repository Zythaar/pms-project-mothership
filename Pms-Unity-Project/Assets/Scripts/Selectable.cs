using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Ship, Station, Minable };
public enum EntityID { Worker, Transporter, Storage, Shipyard };

namespace PMS.UI
{
    public class Selectable : MonoBehaviour
    {
        public EntityType entityType;
        public EntityID entityID;

        public bool IsPointed { get { return isPointed; } private set { isPointed = value; } } 
        public bool IsSelected { get { return isSelected; } private set { isSelected = value; } }

        private bool isPointed;
        private bool isSelected;

        public Color pointedColor;
        public Color selectedColor;
        public Color pointedAndSelectedColor;

        public SpriteRenderer circle;


        private void Start()
        {
            SelectionController controller = GameManager.singleton.selectionController;
            controller.onPointed += OnPointed;
            controller.onDePointed += OnDePointed;
            controller.onSelected += OnSelected;
            controller.onDeselected += OnDeselected;

            circle.enabled = false;

            
        }

        private void OnDisable()
        {
            //SelectionController controller = GameManager.singleton.selectionController;
            //controller.onPointed -= OnPointed;
            //controller.onDePointed -= OnDePointed;
            //controller.onSelected -= OnSelected;
            //controller.onDeselected -= OnDeselected;
        }

        public void Select()
        {

        }


        public void OnPointed()
        {
            isPointed = true;

            ChangeCircle();
        }

        public void OnDePointed()
        {
            isPointed = false;

            ChangeCircle();
        }

        public void OnSelected()
        {
            isSelected = true;


            ChangeCircle();
        }

        public void OnDeselected()
        {
            isSelected = false;


            ChangeCircle();
        }

        void ChangeCircle()
        {
            if (isSelected && isPointed)
            {
                circle.material.color = pointedAndSelectedColor;
            }
            else if (isSelected)
            {
                circle.material.color = selectedColor;
            }
            else if (isPointed)
            {
                circle.material.color = pointedColor;
            }

            circle.enabled = isPointed || isSelected;
        }
    }

}