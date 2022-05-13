export function display(str: string) {
    let node = document.createElement('div');
    node.innerHTML = str;
    document.body.appendChild(node);
}