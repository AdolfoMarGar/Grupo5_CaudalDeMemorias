using UnityEngine;

public class TreeInteractable : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        if (!IsActive()) return;

        Interact();
    }
    public override void Interact()
    {
        base.Interact();

        // Aquí puedes añadir lógica específica adicional para los objetos clickeables si lo necesitas
        Debug.Log($"Clic detectado directamente en el objeto ID: {GetId()}");
    }
}
