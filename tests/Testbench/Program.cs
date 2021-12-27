/*
 * PURELY HERE TO TEST CODE GENERATION AND ANALYZERS.
 * This project is the target project when debugging the main Vogen project.
 *
 * To debug, Select Vogen as the active project, and select 'Roslyn' as the target,
 * and the press F5. That will start the debugger and Vogen will use this project
 * to analyze and generate types for.
 */

using Testbench;

CallMe(MyVo.From(123));

void CallMe(MyVo customerId)
{
    int _ = customerId.Value;
}

