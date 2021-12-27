/*
 * PURELY HERE TO TEST CODE GENERATION AND ANALYZERS.
 * This project is the target project when debugging the main Vogen project.
 *
 * To debug, Select Vogen as the active project, and select 'Roslyn' as the target,
 * and the press F5. That will start the debugger and Vogen will use this project
 * to analyze and generate types for.
 */

using Testbench;

// uncomment this to fail the build (as the analyzer will say that Value Objects can't be constructed with new())
// MyVo v = new();
Console.ReadLine();