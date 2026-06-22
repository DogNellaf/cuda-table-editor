#include <stdio.h>

__device__ void partition(char *data, int left, int right, char pivot, int &nleft, int &nright) {
    while (left <= right) {
        while (data[left] < pivot) left++;
        while (data[right] > pivot) right--;
        if (left <= right) {
            char temp = data[left];
            data[left] = data[right];
            data[right] = temp;
            left++;
            right--;
        }
    }
    nleft = left;
    nright = right;
}

__global__ void quicksort(char *data, int left, int right) {
    int nleft, nright;
    if (left < right) {
        char pivot = data[left];
        partition(data, left, right, pivot, nleft, nright);
        if (left < nright) {
            quicksort<<<1, 1>>>(data, left, nright);
        }
        if (nleft < right) {
            quicksort<<<1, 1>>>(data, nleft, right);
        }
    }
}

void launch_quicksort(char *data, int count) {
    quicksort<<<1, 1>>>(data, 0, count - 1);
    cudaDeviceSynchronize();
}

int main() {
    char host_data[] = "bananaappleorangegrapekiwi";
    int length = strlen(host_data);

    char *dev_data;
    cudaMalloc((void **)&dev_data, length * sizeof(char));
    cudaMemcpy(dev_data, host_data, length * sizeof(char), cudaMemcpyHostToDevice);

    launch_quicksort(dev_data, length);

    cudaMemcpy(host_data, dev_data, length * sizeof(char), cudaMemcpyDeviceToHost);
    printf("Sorted string: %s\n", host_data);

    cudaFree(dev_data);

    return 0;
}