using System.Collections.Generic;
using Maniac.Utils;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scenes.WCFTest
{
    public class WFCCellVisualizer : MonoBehaviour
    {
        private WFCVisualizer _wfcVisualizer => Locator<WFCVisualizer>.Instance;
        private WFCCell _cell;
        [SerializeField] private SpriteRenderer mainSpriteRenderer;
        [SerializeField] private List<SpriteRenderer> subSpriteRenderers;

        public void Setup(WFCCell cell)
        {
            _cell = cell;

            cell.FinalSprite.Subscribe(value =>
            {
                if (value != null)
                {
                    mainSpriteRenderer.sprite = value;
                    foreach (var subSpriteRenderer in subSpriteRenderers)
                    {
                        subSpriteRenderer.sprite = null;
                    }
                }
            }).AddTo(this);
            
            cell.PossibleIds.ObserveCountChanged().Subscribe(value =>
            {
                for (int i = 0; i < subSpriteRenderers.Count; i++)
                {
                    bool shouldEnable = i < value;
                    subSpriteRenderers[i].gameObject.SetActive(shouldEnable);
                    
                    if(shouldEnable)
                        subSpriteRenderers[i].sprite = _wfcVisualizer.GetSpriteWithId(cell.PossibleIds[i]);
                }
            }).AddTo(this);
        }
    }
}