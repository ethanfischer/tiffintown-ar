using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookMenu : MonoBehaviour
{
    [SerializeField]
    TMP_Text _bookTitle;
    [SerializeField]
    TMP_Text _description;
    [SerializeField]
    Image _bookImage;
    [SerializeField]
    Sprite[] _bookImageSprites;
    [SerializeField]
    string[] _purchaseURLS;
    [SerializeField]
    OpenLink _purchaseLink;
    
    

    public void SetBook(BookTitle bookTitle)
    {
        if (bookTitle == BookTitle.JosephineBaker)
        {
            _bookTitle.text = "Josephine Baker";
            _description.text = "Josephine Baker is a writer, editor, and publisher. She is the author of the best-selling memoir, “The Secret Life of Bees.” She has also written several other books, including “The Art of Being a Woman.” Josephine Baker is a highly respected figure in the literary world and has received numerous awards for her work.";
            _bookImage.sprite = _bookImageSprites[(int)BookTitle.JosephineBaker];
            _purchaseLink.Link = _purchaseURLS[(int)BookTitle.JosephineBaker];
        }
        else if (bookTitle == BookTitle.RuthHarkness)
        {
            _bookTitle.text = "Ruth Harkness";
            _description.text = "Ruth Harkness is a writer, editor, and publisher. She is the author of the best-selling memoir, “The Secret Life of Bees.” She has also written several other books, including “The Art of Being a Woman.” Ruth Harkness is a highly respected figure in the literary world and has received numerous awards for her work.";
            _bookImage.sprite = _bookImageSprites[(int)BookTitle.RuthHarkness];
            _purchaseLink.Link = _purchaseURLS[(int)BookTitle.RuthHarkness];
        }
    }
}
