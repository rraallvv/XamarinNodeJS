#include <stdint.h>
#include "CLib.h"

int32_t clib_add(int32_t left, int32_t right)
{
    return clib_add_internal(left, right);
}
