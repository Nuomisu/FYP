var speed:float=10;

function Update () {
transform.position.x -= Input.GetAxis("Horizontal") * Time.deltaTime * speed;
transform.position.y += Input.GetAxis("Vertical")* Time.deltaTime * speed/2;
}