#include <cuda_runtime.h>
#include <device_launch_parameters.h>
#include <cstdio>

extern "C" __global__ void EvaluateExpression(const char* expression, double* result)
{
    double res = 0.0;
    double num = 0.0;
    char op = '+';
    int i = 0;

    while (expression[i] != '\0')
    {
        if (expression[i] >= '0' && expression[i] <= '9')
        {
            num = num * 10 + (expression[i] - '0');
        }
        else if (expression[i] == '+' || expression[i] == '-' || expression[i] == '*' || expression[i] == '/')
        {
            switch (op)
            {
            case '+': res += num; break;
            case '-': res -= num; break;
            case '*': res *= num; break;
            case '/': res /= num; break;
            }
            num = 0.0;
            op = expression[i];
        }
        i++;
    }

    switch (op)
    {
    case '+': res += num; break;
    case '-': res -= num; break;
    case '*': res *= num; break;
    case '/': res /= num; break;
    }

    *result = res;
}

int main()
{
    // This main function is just for testing
    const char* expression = "3+5*2-4/2";
    double result;
    EvaluateExpression << <1, 1 >> > (expression, &result);
    cudaDeviceSynchronize();
    printf("Result: %f\n", result);
    return 0;
}