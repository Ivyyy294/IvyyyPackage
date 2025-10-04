using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ivyyy
{
    public class DialogGraph : EditorWindow
    {
	    private DialogGraphView dialogGraphView;
	    private DialogContainer cachedContainer;

	    public static void OpenDialogGraphWindow(DialogContainer asset)
	    {
		    if (asset != null)
		    {
			    var window = GetWindow <DialogGraph>();
			    string assetName = asset.name;
			    window.titleContent = new GUIContent ("Dialog Graph [" + assetName + "]");
			    window.cachedContainer = asset;
			    window.RequestDataOperation (false);
		    }
		    else
			    EditorUtility.DisplayDialog ("Invalid Asset!", "Please contact Lara!", "OK");
	    }

	    private void OnEnable()
	    {
		    ConstructGraphView();
		    GenerateToolbar();
		    GenerateMiniMap();
		    GenerateBlackBoard();
	    }

	    private void GenerateBlackBoard()
	    {
		    var blackBoard = new Blackboard(dialogGraphView);
		    blackBoard.Add (new BlackboardSection () {title = "Exposed Properties"});
		    blackBoard.SetPosition (new Rect (10, 200, 200, 140));
		    blackBoard.addItemRequested = blackBoard1 => {dialogGraphView.AddPropertyToBlackBoard("VALUE");};
		    blackBoard.editTextRequested = (blackBoard1, element, newValue) =>
		    {
			    BlackboardField blackboardField = (BlackboardField)element;

			    string oldName = blackboardField.text;

			    if (dialogGraphView.blackBoardProperties.data.Any(x=>x.name == newValue))
			    {
				    EditorUtility.DisplayDialog ("Error", "This property name already exists, please chose another one!", "OK");
				    return;
			    }

			    blackboardField.text = newValue;
			    dialogGraphView.blackBoardProperties.ChangeData (newValue, blackboardField.typeText);
			    dialogGraphView.RefreshBlackBoardProperties();
		    };

		    dialogGraphView.Add (blackBoard);
		    dialogGraphView.blackboard = blackBoard;
	    }

	    private void OnDisable()
	    {
		    rootVisualElement.Remove (dialogGraphView);
	    }

	    private void GenerateMiniMap()
	    {
		    var miniMap = new MiniMap {anchored = true };
		    miniMap.SetPosition (new Rect (10, 30, 200, 140));
		    dialogGraphView.Add (miniMap);
	    }


	    private void ConstructGraphView()
	    {
		    dialogGraphView = new DialogGraphView {name = "Dialog Graph"};
		    dialogGraphView.StretchToParentSize();
		    rootVisualElement.Add (dialogGraphView);
	    }

	    private void GenerateToolbar()
	    {
		    var toolbar = new Toolbar();

		    toolbar.Add (new Button (clickEvent:() => RequestDataOperation(true)) {text = "Save Data"});

		    rootVisualElement.Add (toolbar);
	    }

	    private void RequestDataOperation(bool save)
	    {
		    if (cachedContainer == null)
		    {
			    EditorUtility.DisplayDialog ("Invalid asset!", "Please contact Lara!", "OK");
			    return;
		    }

		    var saveUtility = GraphSaveUtility.GetInstance (dialogGraphView);

		    if (save)
			    saveUtility.SaveGraph (cachedContainer);
		    else
			    saveUtility.LoadGraph (cachedContainer);
	    }
    }
   

}