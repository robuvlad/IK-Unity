using Assets.Scripts.StewartPlatform.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.StewartPlatform.simulation
{
    public class PanelDrawing
    {
        private GameObject[] panels;

        public PanelDrawing(GameObject[] panels)
        {
            this.panels = panels;
        }

        public void SetPanelsDisabled()
        {
            for(int i = 0; i < StewartPlatformIKUtils.NUMBER_OF_LEGS; i++)
            {
                panels[i].SetActive(false);
            }
        }

        public void ShowPanel(int value)
        {
            SetPanelsDisabled();
            panels[value].SetActive(true);
        }
    }
}
