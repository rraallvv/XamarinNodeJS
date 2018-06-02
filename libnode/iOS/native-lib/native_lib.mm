//
//  NodeRunner.mm
//  native-xcode-node-folder
//
//  Created by Jaime Bernardo on 08/03/2018.
//  Copyright © 2018 Janea Systems. All rights reserved.
//

#include "native_lib.h"
#include <NodeMobile/NodeMobile.h>
#include <string>

int startNodeWithArguments(int argument_count, char * argv[]) {
	int c_arguments_size=0;

	//Compute byte size need for all arguments in contiguous memory.
	for (int i = 0; i < argument_count; i++)
	{
		c_arguments_size+=strlen(argv[i]);
		c_arguments_size++; // for '\0'
	}

	//Stores arguments in contiguous memory.
	char* args_buffer=(char*)calloc(c_arguments_size, sizeof(char));

	//Adjacent argv to pass into node.
	char* adj_argv[argument_count];

	//To iterate through the expected start position of each argument in args_buffer.
	char* current_args_position=args_buffer;

	//Populate the args_buffer and adj_argv.
	for (int i = 0; i < argument_count; i++)
	{
		const char* current_argument=argv[i];

		//Copy current argument to its expected position in args_buffer
		strncpy(current_args_position, current_argument, strlen(current_argument));

		//Save current argument start position in adj_argv and increment argc.
		adj_argv[i]=current_args_position;

		//Increment to the next argument's expected position.
		current_args_position+=strlen(current_args_position)+1;
	}

	//Start node, with argc and adj_argv.
	return node_start(argument_count,adj_argv);
}
