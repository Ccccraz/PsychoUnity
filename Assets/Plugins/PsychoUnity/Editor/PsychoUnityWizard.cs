using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PsychoUnity.Editor
{
    public class PsychoUnityWizard : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("PsychoUnity/Wizard")]
        public static void ShowExample()
        {
            PsychoUnityWizard wnd = GetWindow<PsychoUnityWizard>();
            wnd.titleContent = new GUIContent("PsychoUnityWizard");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);
        }
    }
}
