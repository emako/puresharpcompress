string[] filePaths = Directory.GetFiles("../../../../lib", "*", SearchOption.AllDirectories);

Parallel.ForEach(filePaths, filePath =>
{
    Console.WriteLine($"Processing: {filePath}, Thread: {Task.CurrentId}");

    Pura.PuraIt(filePath);
});
