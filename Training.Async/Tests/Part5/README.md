# Section description

Part 5 presents async programming good practices

- Do not block on async code
- Do not use async void
- Do not pretend async
- Do not continiue on main thread unnecessarily
- Try to use Task.Run instead of Task.Factory.StartNew
- Be aware of risk using Task.Run in ASP.NET
- Use async/await for any I/O operation if possible
- Do not await in loop
- Be aware ForEach loop with async lambda is equal to async void 
- Be aware of mutating non-thread-safe reference type instance in async method
- Try to use async methods instaed sync counterparts
- Await in try/catch block
- If fire-and-forget task created intenationally use "discards" 
- Use async eliding for simple passthrough or overload method
- Do not use async eliding with using statements
- Do not use async/await in long running task 
- Learn and use C# new versions constructs such as IAsyncEnumerable, ValueTask etc. 

