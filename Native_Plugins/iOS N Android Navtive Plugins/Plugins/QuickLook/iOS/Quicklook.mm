#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>
#import <QuickLook/QuickLook.h>
#import <ARKit/ARKit.h>

typedef void (*INT_CALLBACK)(int);
typedef void (*VOID_CALLBACK)(void);


/*
 * credit : https://medium.com/ios-os-x-development/ios-using-quicklook-for-fun-and-profit-d9a338e2f7fb
 *  Quicklook Preview Item
 */
@interface PreviewItem : NSObject <QLPreviewItem>
@property(readonly, nullable, nonatomic) NSURL    *previewItemURL;
@property(readonly, nullable, nonatomic) NSString *previewItemTitle;
@end@implementation PreviewItem- (instancetype)initPreviewURL:(NSURL *)docURL
                     WithTitle:(NSString *)title {
    self = [super init];
    if (self) {
        _previewItemURL = [docURL copy];
        _previewItemTitle = [title copy];
    }
    return self;
}@end
/***/

/*
 * credit : https://medium.com/ios-os-x-development/ios-using-quicklook-for-fun-and-profit-d9a338e2f7fb
 *  QuickLook Datasource for render 3d modals
 */
@interface DataSource : NSObject <QLPreviewControllerDataSource>
@property (strong, nonatomic) PreviewItem *item;
@end@implementation DataSource
- (instancetype)initWithPreviewItem:(PreviewItem *)item {
    self = [super init];
    if (self) {
        _item = item;
    }
    return self;
}
- (NSInteger)numberOfPreviewItemsInPreviewController:(QLPreviewController *)controller {
    return 1;
}
- (id<QLPreviewItem>)previewController:(QLPreviewController *)controller previewItemAtIndex:(NSInteger)index {
    return self.item;
}@end
/***/

@interface Quicklook : NSObject<QLPreviewControllerDelegate>
{
    VOID_CALLBACK quickLookWillDismissCallBack;
    //VOID_CALLBACK quickLookDidDismissCallBack;
}
@end

@implementation Quicklook

static Quicklook *_sharedInstance;

+(Quicklook*) sharedInstance
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        NSLog(@"creating Quicklook sharedInstance");
        _sharedInstance = [[Quicklook alloc] init];
    });
    return _sharedInstance;
}

-(id)init
{
    self = [super init];
    if(self){
        [self initHelper];
    }
    return self;
}


-(void)initHelper
{
    NSLog(@"initHelper called");
}


- (void)previewControllerWillDismiss:(QLPreviewController *)controller{
    NSLog(@"$$$$ Will Dismiss callback !!!!");
    if(quickLookWillDismissCallBack != nil){
        quickLookWillDismissCallBack();
    }
}

//- (void)previewControllerDidDismiss:(QLPreviewController *)controller{
//    NSLog(@"$$$$ About to send a Did Dismiss callback !!!!");
//    //self.closingCallback();
//}

-(void)ShowQuickLook:(const char*) filePath quickLookDismissCallBack:(VOID_CALLBACK)quickLookDismissCallBack
{
    NSLog(@"$$$$ ShowQuickLook Called !!!!");
    NSURL *fileUrl = [NSURL fileURLWithPath:[NSString stringWithUTF8String:filePath]];
    
    PreviewItem *item =
    [[PreviewItem alloc] initPreviewURL:fileUrl
                              WithTitle:@"Made By Atul"];
    
    DataSource *dataSource =
    [[DataSource alloc] initWithPreviewItem:item];
    
    /*ARQuickLookPreviewItem* previewItem = [[ARQuickLookPreviewItem alloc]initWithFileAtURL:fileUrl];*/
    
    
    QLPreviewController *previewController = [[QLPreviewController alloc] init];
    
    previewController.dataSource = dataSource;
    
    previewController.delegate = self;
    
    quickLookWillDismissCallBack = quickLookDismissCallBack;
            
    UIViewController *rootViewController = UnityGetGLViewController();
        
    [rootViewController presentViewController:previewController animated:YES completion:nil];
    
    NSLog(@"$$$$ ShowQuickLook ended !!!!");
}

@end

extern "C"
{
void _QuickLook( const char* file , VOID_CALLBACK quickLookDismissCallBack){
    [[Quicklook sharedInstance] ShowQuickLook:file quickLookDismissCallBack:quickLookDismissCallBack];
}
}
