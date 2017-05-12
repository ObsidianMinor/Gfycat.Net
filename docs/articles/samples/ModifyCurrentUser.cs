var currentUser = await gfycat.GetCurrentUserAsync(); // Get the current user
await currentUser.ModifyCurrentUserAsync(new GfycatOperation[] { // Modify the current user
    new GfycatOperation(Operation.Add, OperationPath.ProfileUrl, "https://github.com/MarkusGordathian"), // Describes how we want to modify the user (add a profile URL directed to my GitHub page)"
    new GfycatOperation(Operation.Remove, OperationPath.Description, null), // Remove my description (bio)
    new GfycatOperation(Operation.Add, OperationPath.Name, "Mark Gross") // Change my display name
});

await currentUser.UploadProfilePictureAsync(File.OpenRead("profile.png")); // Upload a (new) profile picture