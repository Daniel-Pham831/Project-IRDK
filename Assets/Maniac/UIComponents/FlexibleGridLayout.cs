using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Maniac.UIComponents
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            Uniform,
            Width,
            Height,
            FixedRows,
            FixedColumns,
        }
        
        [SerializeField] private FitType fitType;
        
        [ShowIf("@fitType == FitType.FixedRows")]
        [DisableIf("@fitType == FitType.FixedColumns")]
        [MinValue(1)]
        [SerializeField] private int rows;
        
        [ShowIf("@fitType == FitType.FixedColumns")]
        [DisableIf("@fitType == FitType.FixedRows")]
        [MinValue(1)]
        [SerializeField] private int columns;
        
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private Vector2 spacing;

        private bool fitX;
        private bool fitY;
        
        public override void CalculateLayoutInputVertical()
        {
            if (fitType is FitType.Width or FitType.Height or FitType.Uniform)
            {
                fitX = true;
                fitY = true;
                float sqr = Mathf.Sqrt(transform.childCount);
                rows = Mathf.CeilToInt(sqr);
                columns = Mathf.CeilToInt(sqr);
            }

            if (fitType is FitType.Width or FitType.FixedColumns)
            {
                rows = Mathf.CeilToInt(transform.childCount / ((float)columns));
            }

            if (fitType is FitType.Height or FitType.FixedRows)
            {
                columns = Mathf.CeilToInt(transform.childCount / ((float)rows));
            }


            var rect = rectTransform.rect;
            float parentWidth = rect.width;
            float parentHeight = rect.height;

            float cellWidth = (parentWidth / ((float)columns)) - ((spacing.x / (float)columns) * (columns - 1)) - (padding.left /(float)columns )- (padding.right /(float)columns );
            float cellHeight = (parentHeight / ((float)rows)) - ((spacing.y / (float)rows) * (rows - 1))- (padding.top /(float)rows )- (padding.bottom /(float)rows );

            cellSize.x = fitX ? cellWidth : cellSize.x;
            cellSize.y = fitY ? cellHeight : cellSize.y;

            int columnCount = 0;
            int rowCount = 0;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                rowCount = i / columns;
                columnCount = i % columns;

                var item = rectChildren[i];

                var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;
                
                SetChildAlongAxis(item,0,xPos,cellSize.x);
                SetChildAlongAxis(item,1,yPos,cellSize.y);
            }
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
        }

        public override void SetLayoutHorizontal()
        {
            
        }

        public override void SetLayoutVertical()
        {
            
        }
    }
}