#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

typedef void (*RESULT_CALLBACK)(BOOL,const char*);

@interface NativeShare : NSObject
{
    RESULT_CALLBACK shareResultCallBack;
    UIPopoverController *popover;
}

@end

@implementation NativeShare

static NativeShare *_sharedInstance;


+(NativeShare*) sharedInstance
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        NSLog(@"Creating NativeShare Shared Instance !!!");
        _sharedInstance = [[NativeShare alloc]init];
    });
    return _sharedInstance;
}


-(id)init
{
    self = [super init];
    
    if(self)
    {
        [self initHelper];
    }
    return self;
}

-(void)initHelper
{
    NSLog(@"initHelper called $$$");
}

-(void)_NativeShare_Share:(const char*)file caption:(const char*) caption_in resultCallBack:(RESULT_CALLBACK) resultCallBack
{
    NSMutableArray *shareableItems = [NSMutableArray new];
        
    NSString *filePath = [NSString stringWithUTF8String:file];
    if( filePath != nil )
    {
        [shareableItems addObject:[NSURL fileURLWithPath:filePath]];
    }
    else
    {
        NSLog(@"filePath is null $$$$");
    }
    
    if(caption_in != nil && strlen( caption_in ) > 0 )
    {
        [shareableItems addObject:[NSString stringWithUTF8String:caption_in]];
    }
    
    shareResultCallBack = resultCallBack;

    UIActivityViewController *activity = [[UIActivityViewController alloc] initWithActivityItems:shareableItems  applicationActivities:nil];

    activity.completionWithItemsHandler = ^(NSString *activityType, BOOL completed, NSArray *returnedItems, NSError *activityError ){
        NSLog(@"Activity %@ Completed : %d",activityType,completed);
        if(shareResultCallBack != nil)
        {
            shareResultCallBack(completed,[activityType UTF8String]);
        }else
        {
            NSLog(@"shareResultCallBack is null $$$$");
        }
    };
    
    UIViewController *rootViewController = UnityGetGLViewController();
    
    if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone ) // iPhone
    {
        [rootViewController presentViewController:activity animated:YES completion:^{NSLog(@"share shown $$$$");}];
    }
    else // iPad
    {
        popover = [[UIPopoverController alloc] initWithContentViewController:activity];
        [popover presentPopoverFromRect:CGRectMake( rootViewController.view.frame.size.width / 2, rootViewController.view.frame.size.height / 2, 1, 1 ) inView:rootViewController.view permittedArrowDirections:0 animated:YES];
    }
}

@end

extern "C"
{

void IOSNativeShare( const char* file, const char* caption, RESULT_CALLBACK resultCallBack)
{
    [[NativeShare sharedInstance] _NativeShare_Share:file caption:caption resultCallBack:resultCallBack];
}
   
}