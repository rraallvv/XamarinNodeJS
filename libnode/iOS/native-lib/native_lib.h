//
//  NodeRunner.h
//  native-xcode-node-folder
//
//  Created by Jaime Bernardo on 08/03/2018.
//  Copyright © 2018 Janea Systems. All rights reserved.
//

#ifndef NATIVE_LIB_H
#define NATIVE_LIB_H

#ifdef __cplusplus
extern "C" {
#endif

int clib_add(int left, int right) {
	return left + right;
}

int startNodeWithArguments(int argument_count, char * argv[]);

#ifdef __cplusplus
}
#endif

#endif
