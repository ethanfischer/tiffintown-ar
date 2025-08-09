using UnityEngine;

public class Book : MonoBehaviour
{
	[SerializeField]
	BookTitle _bookTitle;
	
	public void OpenBookPage()
	{
        MenuManager.Instance.GoBook(_bookTitle);
	}
}

public enum BookTitle
{
	JosephineBaker,
	RuthHarkness
}
