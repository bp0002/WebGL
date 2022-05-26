export function display(str: string) {
    let node = document.createElement('span');
    node.innerHTML = str;
    document.body.style.whiteSpace = "pre";
    document.body.style.fontFamily = "serif";
    document.body.appendChild(node);
}