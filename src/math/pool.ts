
const float32arrayPool: Float32Array[] = [];
const float32arrayPool3: Float32Array[] = [];
const float32arrayPool4: Float32Array[] = [];
const float32arrayPool16: Float32Array[] = [];
export function createFloat32Array(size: number): Float32Array {
    let result: Float32Array | undefined;
    if (size == 3) {
        result = float32arrayPool3.pop();
    } else if (size == 4) {
        result = float32arrayPool4.pop();
    } else if (size == 16) {
        result = float32arrayPool16.pop();
    } else {
        let len = float32arrayPool.length;
        if (len > 0) {
            let index = -1;
            for (let i = len - 1; i >= 0; i--) {
                if (float32arrayPool[i].length == size) {
                    index = i;
                    result = float32arrayPool[i];
                    break;
                }
            }

            if (len > 1) {
                float32arrayPool[index] = float32arrayPool[len - 1];
            }
            float32arrayPool.length = len - 1;
        }
    }

    if (!result) {
        result = new Float32Array(size);
    }

    return result;
}
export function recycleFloat32Array(data: Float32Array) {
    let len = data.length;
    if (len == 3) {
        float32arrayPool3.push(data);
    } else if (len == 4) {
        float32arrayPool4.push(data);
    } else if (len == 16) {
        float32arrayPool16.push(data);
    } else {
        float32arrayPool.push(data);
    }
}
