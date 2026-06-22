
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <ctime>
#include <stdio.h>
#include <vector>
#include <limits>
#include <algorithm>

#include <Windows.h>

extern "C" __global__ void swapOnKernel(double *a, int size)
{
    int i = blockDim.x * blockIdx.x + threadIdx.x * 2;
	double cacheFirst;
	double cacheSecond;
	double cacheThird;

    for (int j = 0; j < size/2 + 1; j++) {

	    if(i+1 < size) {
		    cacheFirst = a[i];
		    cacheSecond = a[i+1];

		    if(cacheFirst > cacheSecond) {
			    double temp = cacheFirst;
			    a[i] = cacheSecond;
			    cacheSecond = a[i+1] = temp;
		    }
	    }

	    if(i+2 < size) {
		    cacheThird = a[i+2];
		    if(cacheSecond > cacheThird) {
			    double temp = cacheSecond;
			    a[i+1] = cacheThird;
			    a[i+2] = temp;
		    }
	    }

        __syncthreads();
    }

}