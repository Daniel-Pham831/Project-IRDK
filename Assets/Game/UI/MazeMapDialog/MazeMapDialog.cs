using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Maze;
using Game.Scenes.WCFTest;
using Game.UI.MazeMapDialog;
using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.Utils;
using UnityEngine.UI;

namespace Game
{
    public class MazeMapDialog : BaseUI
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private MazeSystem _mazeSystem => Locator<MazeSystem>.Instance;
        private MazeConfig _mazeConfig => _dataBase.GetConfig<MazeConfig>();

        [SerializeField] private CellVisualizer cellVisualizerPrefab;
        [SerializeField] private Transform cellVisualizerParent;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        private List<CellVisualizer> _cellVisualizers = new List<CellVisualizer>();
        
        [SerializeField] private MazeNoticeItem mazeNoticeItemPrefab;
        [SerializeField] private Transform mazeNoticeItemParent;

        [SerializeField] private LanguageItem startCellLangItem;
        [SerializeField] private LanguageItem endCellLangItem;
        [SerializeField] private LanguageItem youAreHereLangItem;
        [SerializeField] private LanguageItem traderLangItem;

        public override async void OnSetup(object parameter = null)
        {
            base.OnSetup(parameter);

            await SetupMazeMap();
            await SetupMazeNotice();
        }

        private async UniTask SetupMazeNotice()
        {
            var youAreHereNoticeItem = Instantiate(mazeNoticeItemPrefab, mazeNoticeItemParent);
            youAreHereNoticeItem.Setup(youAreHereLangItem.GetCurrentLanguageText(),_mazeConfig.YouAreHereColor);
            
            var startNoticeItem = Instantiate(mazeNoticeItemPrefab, mazeNoticeItemParent);
            startNoticeItem.Setup(startCellLangItem.GetCurrentLanguageText(),_mazeConfig.StartCellColor);
            
            var endNoticeItem = Instantiate(mazeNoticeItemPrefab, mazeNoticeItemParent);
            endNoticeItem.Setup(endCellLangItem.GetCurrentLanguageText(),_mazeConfig.EndCellColor);
            
            var traderNoticeItem = Instantiate(mazeNoticeItemPrefab, mazeNoticeItemParent);
            traderNoticeItem.Setup(traderLangItem.GetCurrentLanguageText(),Color.white);
            traderNoticeItem.SetSprite(_mazeConfig.TraderSmallSprite);
        }

        private async Task SetupMazeMap()
        {
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            gridLayoutGroup.constraintCount = _mazeSystem.CurrentMaze.Dimension.y;
            await ConstructCellVisualizers();
        }

        private async UniTask ConstructCellVisualizers()
        {
            foreach (var cell in _mazeSystem.CurrentMaze.Cells)
            {
                var cellVisualizer = Instantiate(cellVisualizerPrefab, cellVisualizerParent);
                cellVisualizer.Setup(cell);
                _cellVisualizers.Add(cellVisualizer);
            }

            await UniTask.DelayFrame(1);
            SetupStartAndEndCellVisualizers();
        }

        private void SetupStartAndEndCellVisualizers()
        {
            var startCellVisualizer = Instantiate(cellVisualizerPrefab, cellVisualizerParent);
            startCellVisualizer.Setup(_mazeSystem.CurrentMaze.StartCell);
            startCellVisualizer.SetIgnoreLayout(true);
            var startCellNeighbour = GetNeighbourNextToStartCell();
            var startNeighbourRect = startCellNeighbour.GetComponent<RectTransform>();
            var startRect = startCellVisualizer.GetComponent<RectTransform>();
            startRect.localPosition = startNeighbourRect.localPosition + Vector3.left * startRect.rect.height;
            
            var endCellVisualizer = Instantiate(cellVisualizerPrefab, cellVisualizerParent);
            endCellVisualizer.Setup(_mazeSystem.CurrentMaze.EndCell);
            endCellVisualizer.SetIgnoreLayout(true);
            var endCellNeighbour = GetNeighbourNextToEndCell();
            var endNeighbourRect = endCellNeighbour.GetComponent<RectTransform>();
            var endRect = endCellVisualizer.GetComponent<RectTransform>();
            endRect.localPosition = endNeighbourRect.localPosition + Vector3.right * endRect.rect.height;
            
            _cellVisualizers.Add(startCellVisualizer);
            _cellVisualizers.Add(endCellVisualizer);
        }

        private CellVisualizer GetNeighbourNextToStartCell()
        {
            foreach (var cellVisualizer in _cellVisualizers)
            {
                if(cellVisualizer.CellModel.Position == _mazeSystem.CurrentMaze.StartCell.Position + Vector2.right)
                    return cellVisualizer;
            }

            return null;
        }
        
        private CellVisualizer GetNeighbourNextToEndCell()
        {
            foreach (var cellVisualizer in _cellVisualizers)
            {
                if(cellVisualizer.CellModel.Position == _mazeSystem.CurrentMaze.EndCell.Position + Vector2.left)
                    return cellVisualizer;
            }

            return null;
        }
    }
}
