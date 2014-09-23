
var speed : float = 0.1;

function Update () {
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
	var touchPosition:Vector2 = Input.GetTouch(0).deltaPosition;
    transform.Translate (-touchPosition.x * speed, -touchPosition.y * speed, 0);
    }
}