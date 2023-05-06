using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class MazeNoticeItem : MonoBehaviour
    {
        [SerializeField] private Image mainImage;
        [SerializeField] private TMP_Text mainText;
        
        public void Setup(string text, Color color)
        {
            mainText.text = text;
            mainImage.color = color;
            mainImage.sprite = null;
        }
        
        public void SetSprite(Sprite sprite)
        {
            mainImage.sprite = sprite;
        }
    }
}