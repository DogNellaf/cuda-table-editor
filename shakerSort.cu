extern "C" __global__ void shekerSort(double* arr, int n) { // Исправлено на 'double*'
    int left = 0;
    int right = n - 1;
    int flag = 1;
    while ((left < right) && flag > 0)
    {
        flag = 0;
        for (int i = left; i < right; i++)
        {
            if (arr[i] > arr[i+1]) // Заменены mass на arr
            {
                double t = arr[i];
                arr[i] = arr[i+1];
                arr[i+1] = t;
                flag = 1;
            }
        }
        right--;
        for (int i = right; i > left; i--)
        {
            if (arr[i-1] > arr[i]) // Заменены mass на arr
            {
                double t = arr[i];
                arr[i] = arr[i-1];
                arr[i-1] = t;
                flag = 1;
            }
        }
        left++;
        if(flag == 0) break;
    }
}