//
//  iOSBridge.m
//  iOSBridge
//
//  Created by Supersonic.
//  Copyright (c) 2015 Supersonic. All rights reserved.
//

#import "iOSBridge.h"
#import <UIKit/UIKit.h>

// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

#define BANNER_POSITION_TOP 1
#define BANNER_POSITION_BOTTOM 2

#ifdef __cplusplus
extern "C" {
#endif
    
    extern void UnitySendMessage( const char *className, const char *methodName, const char *param );
    
#ifdef __cplusplus
}
#endif

@interface iOSBridge ()
{
    ISBannerView* _bannerView;
    NSInteger _position;
    UIViewController* _bannerViewController;
}

@end

@implementation iOSBridge

char *const IRONSOURCE_EVENTS = "IronSourceEvents";

+ (iOSBridge *)start {
    static iOSBridge *instance;
    static dispatch_once_t onceToken;
    dispatch_once( &onceToken,
                  ^{
                      instance = [iOSBridge new];
                  });
    
    return instance;
}

- (instancetype)init {
    if(self = [super init]){
        [IronSource setRewardedVideoDelegate:self];
        [IronSource setInterstitialDelegate:self];
        [IronSource setISDemandOnlyInterstitialDelegate:self];
        [IronSource setISDemandOnlyRewardedVideoDelegate:self];
        [IronSource setRewardedInterstitialDelegate:self];
        [IronSource setOfferwallDelegate:self];
        [IronSource setBannerDelegate:self];
        
        _bannerView = nil;
        _bannerViewController = nil;
        _position = BANNER_POSITION_BOTTOM;
        
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(orientationChanged:)
                                                     name:UIDeviceOrientationDidChangeNotification object:nil];
    }
    
    return self;
}

- (void)setPluginDataWithType:(NSString *)pluginType pluginVersion:(NSString *)version pluginFrameworkVersion:(NSString *)frameworkVersion {
    [ISConfigurations getConfigurations].plugin = pluginType;
    [ISConfigurations getConfigurations].pluginVersion = version;
    [ISConfigurations getConfigurations].pluginFrameworkVersion = frameworkVersion;
}

#pragma mark Base API

- (void)setAge:(NSInteger)age {
    [IronSource setAge:age];
}

- (void)setGender:(NSString *)gender {
    if([gender caseInsensitiveCompare:@"male"] == NSOrderedSame)
        [IronSource setGender:IRONSOURCE_USER_MALE];
    
    else if([gender caseInsensitiveCompare:@"female"] == NSOrderedSame)
        [IronSource setGender:IRONSOURCE_USER_FEMALE];
    
    else if([gender caseInsensitiveCompare:@"unknown"] == NSOrderedSame)
        [IronSource setGender:IRONSOURCE_USER_UNKNOWN];
}

- (void)setMediationSegment:(NSString *)segment {
    [IronSource setMediationSegment:segment];
}

- (const char *)getAdvertiserId {
    NSString *advertiserId = [IronSource advertiserId];
    
    return MakeStringCopy(advertiserId);
}

- (void)validateIntegration {
    [ISIntegrationHelper validateIntegration];
}

- (void)shouldTrackNetworkState:(BOOL)flag {
    [IronSource shouldTrackReachability:flag];
}

- (BOOL)setDynamicUserId:(NSString *)dynamicUserId {
    return [IronSource setDynamicUserId:dynamicUserId];
}

- (void)setAdaptersDebug:(BOOL)enabled {
    [IronSource setAdaptersDebug:enabled];
}

- (void)setConsent:(BOOL)consent {
    [IronSource setConsent:consent];
}

- (void)setMetaDataWithKey:(NSString *)key value:(NSString *)value {
    [IronSource setMetaDataWithKey:key value:value];
}

#pragma mark Init SDK

- (void)setUserId:(NSString *)userId {
    [IronSource setUserId:userId];
}

- (void)initWithAppKey:(NSString *)appKey {
    [IronSource initWithAppKey:appKey];
}

- (void)initWithAppKey:(NSString *)appKey adUnits:(NSArray<NSString *> *)adUnits {
    [IronSource initWithAppKey:appKey adUnits:adUnits];
}

- (void)initISDemandOnly:(NSString *)appKey adUnits:(NSArray<NSString *> *)adUnits {
    [IronSource initISDemandOnly:appKey adUnits:adUnits];
}

#pragma mark Rewarded Video API

- (void)showRewardedVideo {
    [IronSource showRewardedVideoWithViewController:[UIApplication sharedApplication].keyWindow.rootViewController];
}

- (void)showRewardedVideoWithPlacement:(NSString *)placementName {
    [IronSource showRewardedVideoWithViewController:[UIApplication sharedApplication].keyWindow.rootViewController placement:placementName];
}

- (const char *) getPlacementInfo:(NSString *)placementName {
    char *res = nil;
    
    if (placementName){
        ISPlacementInfo *placementInfo = [IronSource rewardedVideoPlacementInfo:placementName];
        if(placementInfo){
            NSDictionary *dict = @{@"placement_name": [placementInfo placementName],
                                   @"reward_amount": [placementInfo rewardAmount],
                                   @"reward_name": [placementInfo rewardName]};
            
            res = MakeStringCopy([self getJsonFromObj:dict]);
        }
    }
    
    return res;
}

- (BOOL)isRewardedVideoAvailable {
    return [IronSource hasRewardedVideo];
}

- (BOOL)isRewardedVideoPlacementCapped:(NSString *)placementName {
    return [IronSource isRewardedVideoCappedForPlacement:placementName];
}

- (void)setRewardedVideoServerParameters:(NSDictionary *)params {
    [IronSource setRewardedVideoServerParameters:params];
}

- (void)clearRewardedVideoServerParameters {
    [IronSource clearRewardedVideoServerParameters];
}

#pragma mark Rewarded Video DemanOnly API

- (void)showISDemandOnlyRewardedVideo:(NSString *)instanceId {
    [IronSource showISDemandOnlyRewardedVideo:[UIApplication sharedApplication].keyWindow.rootViewController instanceId:instanceId];
}

- (void)loadISDemandOnlyRewardedVideo:(NSString *)instanceId {
    [IronSource loadISDemandOnlyRewardedVideo:instanceId];
}

- (BOOL)isDemandOnlyRewardedVideoAvailable:(NSString *)instanceId {
    return [IronSource hasISDemandOnlyRewardedVideo:instanceId];
}

#pragma mark Rewarded Video Delegate

- (void)rewardedVideoHasChangedAvailability:(BOOL)available {
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAvailabilityChanged", (available) ? "true" : "false");
}

- (void)didReceiveRewardForPlacement:(ISPlacementInfo *)placementInfo {
    NSDictionary *dict = @{@"placement_reward_amount": placementInfo.rewardAmount,
                           @"placement_reward_name": placementInfo.rewardName,
                           @"placement_name": placementInfo.placementName};
    
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdRewarded", MakeStringCopy([self getJsonFromObj:dict]));
}

- (void)rewardedVideoDidFailToShowWithError:(NSError *)error {
    if (error)
        UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdShowFailed", MakeStringCopy([self parseErrorToEvent:error]));
    else
        UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdShowFailed","");
}

- (void)rewardedVideoDidOpen {
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdOpened", "");
}

- (void)rewardedVideoDidClose {
    [self centerBanner];
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdClosed", "");
}

- (void)rewardedVideoDidStart {
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdStarted", "");
}

- (void)rewardedVideoDidEnd {
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdEnded", "");
}

- (void)didClickRewardedVideo:(ISPlacementInfo *)placementInfo {
    NSDictionary *dict = @{@"placement_reward_amount": placementInfo.rewardAmount,
                           @"placement_reward_name": placementInfo.rewardName,
                           @"placement_name": placementInfo.placementName};
    
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdClicked", MakeStringCopy([self getJsonFromObj:dict]));
}

#pragma mark Rewarded Video DemandOnly Delegate

- (void)rewardedVideoDidLoad:(NSString *)instanceId{
    NSArray *params = @[instanceId];
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdLoadedDemandOnly", MakeStringCopy([self getJsonFromObj:params]));
}

- (void)rewardedVideoDidFailToLoadWithError:(NSError *)error instanceId:(NSString *)instanceId{
    NSArray *params;
    if (error)
        params = @[instanceId, [self parseErrorToEvent:error]];
    else
        params = @[instanceId,@""];
    
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdLoadFailedDemandOnly", MakeStringCopy([self getJsonFromObj:params]));
}

- (void)rewardedVideoAdRewarded:(NSString *)instanceId {
    NSArray *params = @[instanceId];
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdRewardedDemandOnly", MakeStringCopy([self getJsonFromObj:params]));
}

- (void)rewardedVideoDidFailToShowWithError:(NSError *)error instanceId:(NSString *)instanceId {
    
    NSArray *params;
    if (error)
        params = @[instanceId, [self parseErrorToEvent:error]];
    else
        params = @[instanceId,@""];
    
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdShowFailedDemandOnly", MakeStringCopy([self getJsonFromObj:params]));
}

- (void)rewardedVideoDidOpen:(NSString *)instanceId {
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdOpenedDemandOnly", MakeStringCopy(instanceId));
    
}

- (void)rewardedVideoDidClose:(NSString *)instanceId {
    [self centerBanner];
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdClosedDemandOnly", MakeStringCopy(instanceId));
}

- (void)rewardedVideoDidClick:(NSString *)instanceId {
    NSArray *params = @[instanceId];
    UnitySendMessage(IRONSOURCE_EVENTS, "onRewardedVideoAdClickedDemandOnly", MakeStringCopy([self getJsonFromObj:params]));
}

#pragma mark Interstitial API

- (void)loadInterstitial {
    [IronSource loadInterstitial];
}

- (void)showInterstitial {
    [IronSource showInterstitialWithViewController:[UIApplication sharedApplication].keyWindow.rootViewController];
}

- (void)showInterstitialWithPlacement:(NSString *)placementName {
    [IronSource showInterstitialWithViewController:[UIApplication sharedApplication].keyWindow.rootViewController placement:placementName];
}

- (BOOL)isInterstitialReady {
    return [IronSource hasInterstitial];
}

- (BOOL)isInterstitialPlacementCapped:(NSString *)placementName {
    return [IronSource isInterstitialCappedForPlacement:placementName];
}

#pragma mark Interstitial DemandOnly API

- (void)loadISDemandOnlyInterstitial:(NSString *)instanceId {
    [IronSource loadISDemandOnlyInterstitial:instanceId];
}

- (void)showISDemandOnlyInterstitial:(NSString *)instanceId {
    [IronSource showISDemandOnlyInterstitial:[UIApplication sharedApplication].keyWindow.rootViewController instanceId:instanceId];
}

- (BOOL)isISDemandOnlyInterstitialReady:(NSString *)instanceId {
    return [IronSource hasISDemandOnlyInterstitial:instanceId];
}

#pragma mark Interstitial Delegate

- (void)interstitialDidLoad {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdReady", "");
}

- (void)interstitialDidFailToLoadWithError:(NSError *)error {
    if (error)
        UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdLoadFailed", MakeStringCopy([self parseErrorToEvent:error]));
    else
        UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdLoadFailed","");
}

- (void)interstitialDidOpen {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdOpened", "");
}

- (void)interstitialDidClose {
    [self centerBanner];
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdClosed", "");
}

- (void)interstitialDidShow {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdShowSucceeded", "");
}

- (void)interstitialDidFailToShowWithError:(NSError *)error {
    if (error)
        UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdShowFailed", MakeStringCopy([self parseErrorToEvent:error]));
    else
        UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdShowFailed","");
}

- (void)didClickInterstitial {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdClicked", "");
}

#pragma mark Interstitial DemandOnly Delegate

- (void)interstitialDidLoad:(NSString *)instanceId {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdReadyDemandOnly", MakeStringCopy(instanceId));
}

- (void)interstitialDidFailToLoadWithError:(NSError *)error instanceId:(NSString *)instanceId {
    NSArray *parameters;
    if (error)
        parameters = @[instanceId, [self parseErrorToEvent:error]];
    else
        parameters = @[instanceId, @""];
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdLoadFailedDemandOnly",MakeStringCopy([self getJsonFromObj:parameters]));
}

- (void)interstitialDidOpen:(NSString *)instanceId {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdOpenedDemandOnly", MakeStringCopy(instanceId));
}

- (void)interstitialDidClose:(NSString *)instanceId {
    [self centerBanner];
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdClosedDemandOnly", MakeStringCopy(instanceId));
}

- (void)interstitialDidShow:(NSString *)instanceId {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdShowSucceededDemandOnly", MakeStringCopy(instanceId));
}

- (void)interstitialDidFailToShowWithError:(NSError *)error instanceId:(NSString *)instanceId {
    NSArray *parameters;
    if (error)
        parameters = @[instanceId, [self parseErrorToEvent:error]];
    else
        parameters = @[instanceId, @""];
    
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdShowFailedDemandOnly", MakeStringCopy([self getJsonFromObj:parameters]));
}

- (void)didClickInterstitial:(NSString *)instanceId {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdClickedDemandOnly", MakeStringCopy(instanceId));
}

#pragma mark Rewarded Interstitial Delegate

- (void)didReceiveRewardForInterstitial {
    UnitySendMessage(IRONSOURCE_EVENTS, "onInterstitialAdRewarded", "");
}

#pragma mark Offerwall API

- (void)showOfferwall {
    [IronSource showOfferwallWithViewController:[UIApplication sharedApplication].keyWindow.rootViewController];
}

- (void)showOfferwallWithPlacement:(NSString *)placementName {
    [IronSource showOfferwallWithViewController:[UIApplication sharedApplication].keyWindow.rootViewController placement:placementName];
}

- (void)getOfferwallCredits {
    [IronSource offerwallCredits];
}

- (BOOL)isOfferwallAvailable {
    return [IronSource hasOfferwall];
}

#pragma mark Offerwall Delegate

- (void)offerwallHasChangedAvailability:(BOOL)available {
    UnitySendMessage(IRONSOURCE_EVENTS, "onOfferwallAvailable", (available) ? "true" : "false");
}

- (void)offerwallDidShow {
    UnitySendMessage(IRONSOURCE_EVENTS, "onOfferwallOpened", "");
}

- (void)offerwallDidFailToShowWithError:(NSError *)error {
    if (error)
        UnitySendMessage(IRONSOURCE_EVENTS, "onOfferwallShowFailed", MakeStringCopy([self parseErrorToEvent:error]));
    else
        UnitySendMessage(IRONSOURCE_EVENTS, "onOfferwallShowFailed", "");
}

- (void)offerwallDidClose {
    [self centerBanner];
    UnitySendMessage(IRONSOURCE_EVENTS, "onOfferwallClosed", "");
}

- (BOOL)didReceiveOfferwallCredits:(NSDictionary *)creditInfo {
    if(creditInfo)
        UnitySendMessage(IRONSOURCE_EVENTS, "onOfferwallAdCredited", [self getJsonFromObj:creditInfo].UTF8String);
    
    return YES;
}

- (void)didFailToReceiveOfferwallCreditsWithError:(NSError *)error {
    if (error)
        UnitySendMessage(IRONSOURCE_EVENTS, "onGetOfferwallCreditsFailed", MakeStringCopy([self parseErrorToEvent:error]));
    else
        UnitySendMessage(IRONSOURCE_EVENTS, "onGetOfferwallCreditsFailed", "");
}

#pragma mark Banner API

- (void)loadBanner:(NSString *)description width:(NSInteger)width height:(NSInteger)height position:(NSInteger)position placement:(NSString *)placement {
    @synchronized(self) {
        _position = position;
        
        ISBannerSize* size = nil;
        if ([description isEqualToString:@"CUSTOM"]) {
            size = [[ISBannerSize alloc] initWithWidth:width andHeight:height];
        }
        else {
            size = [[ISBannerSize alloc] initWithDescription:description];
        }
        
        _bannerViewController = [UIApplication sharedApplication].keyWindow.rootViewController;
        [IronSource loadBannerWithViewController:_bannerViewController size:size placement:placement];
    }
}

- (void)destroyBanner {
    dispatch_async(dispatch_get_main_queue(), ^{
        @synchronized(self) {
            if (_bannerView != nil) {
                [IronSource destroyBanner:_bannerView];
                _bannerView = nil;
                _bannerViewController = nil;
            }
        }
    });
}

- (void)displayBanner {
    dispatch_async(dispatch_get_main_queue(), ^{
        @synchronized(self) {
            if (_bannerView != nil) {
                [_bannerView setHidden:NO];
            }
        }
    });
}

- (void)hideBanner {
    dispatch_async(dispatch_get_main_queue(), ^{
        @synchronized(self) {
            if (_bannerView != nil) {
                [_bannerView setHidden:YES];
            }
        }
    });
}

- (BOOL)isBannerPlacementCapped:(NSString *)placementName {
    return [IronSource isBannerCappedForPlacement:placementName];
}

#pragma mark Banner Delegate

- (CGPoint)getBannerCenter:(NSInteger)position rootView:(UIView *)rootView {
    CGFloat y;
    if (position == BANNER_POSITION_TOP) {
        y = (_bannerView.frame.size.height / 2);
        if (@available(ios 11.0, *)) {
            y += rootView.safeAreaInsets.top;
        }
    }
    else {
        y = rootView.frame.size.height - (_bannerView.frame.size.height / 2);
        if (@available(ios 11.0, *)) {
            y -= rootView.safeAreaInsets.bottom;
        }
    }
    
    return CGPointMake(rootView.frame.size.width / 2, y);
}

- (void)bannerDidLoad:(ISBannerView *)bannerView {
    dispatch_async(dispatch_get_main_queue(), ^{
        @synchronized(self) {
            _bannerView = bannerView;
            [_bannerView setAccessibilityLabel:@"bannerContainer"];
            
            _bannerView.center = [self getBannerCenter:_position rootView:_bannerViewController.view];
            [_bannerViewController.view addSubview:_bannerView];
            
            UnitySendMessage(IRONSOURCE_EVENTS, "onBannerAdLoaded", "");
        }
    });
}

- (void)bannerDidFailToLoadWithError:(NSError *)error {
    if (error)
        UnitySendMessage(IRONSOURCE_EVENTS, "onBannerAdLoadFailed", MakeStringCopy([self parseErrorToEvent:error]));
    else
        UnitySendMessage(IRONSOURCE_EVENTS, "onBannerAdLoadFailed", "");
}

- (void)didClickBanner {
    UnitySendMessage(IRONSOURCE_EVENTS, "onBannerAdClicked", "");
}

- (void)bannerWillPresentScreen {
    UnitySendMessage(IRONSOURCE_EVENTS, "onBannerAdScreenPresented", "");
}

- (void)bannerDidDismissScreen {
    UnitySendMessage(IRONSOURCE_EVENTS, "onBannerAdScreenDismissed", "");
}

- (void)bannerWillLeaveApplication {
    UnitySendMessage(IRONSOURCE_EVENTS, "onBannerAdLeftApplication", "");
}

- (void)centerBanner {
    dispatch_async(dispatch_get_main_queue(), ^{
        @synchronized(self) {
            if (_bannerView != nil && _bannerViewController != nil) {
                _bannerView.center = [self getBannerCenter:_position rootView:_bannerViewController.view];
            }
        }
    });
}

- (void)orientationChanged:(NSNotification *)notification {
    [self centerBanner];
}

#pragma mark Helper methods

- (void) setSegment:(NSString*) segmentJSON {
    [IronSource setSegmentDelegate:self];
    ISSegment *segment = [[ISSegment alloc] init];
    NSError* error;
    if (!segmentJSON)
        return;
    
    NSData *data = [segmentJSON dataUsingEncoding:NSUTF8StringEncoding];
    if (!data)
        return;
    
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (!dict)
        return;
    
    NSMutableArray *allKeys = [[dict allKeys] mutableCopy];
    for (id key in allKeys)
    {
        NSString* keyString = (NSString*)key;
        NSString *object = [dict objectForKey: keyString];
        if ([keyString isEqualToString:@"age"]){
            segment.age = [object intValue] ;
        }
        else if([keyString isEqualToString:@"gender"]){
            if([object caseInsensitiveCompare:@"male"] == NSOrderedSame)
                segment.gender = IRONSOURCE_USER_MALE ;
            else if([object caseInsensitiveCompare:@"female"] == NSOrderedSame)
                segment.gender = IRONSOURCE_USER_FEMALE;
            
        }
        else if ([keyString isEqualToString:@"level"]){
            segment.level =  [object intValue];
        }
        else if ([keyString isEqualToString:@"isPaying"]){
            segment.paying = [object boolValue];
        }
        else if ([keyString isEqualToString:@"userCreationDate"]){
            NSDate *date = [NSDate dateWithTimeIntervalSince1970: [object longLongValue]/1000];
            segment.userCreationDate = date;
            
        }
        else if ([keyString isEqualToString:@"segmentName"]){
            segment.segmentName = object;
            
        } else if ([keyString isEqualToString:@"iapt"]){
            segment.iapTotal = [object doubleValue];
        }
        else{
            [segment setCustomValue:object forKey:keyString];
        }
        
    }
    
    [IronSource setSegment:segment];
}

- (void)didReceiveSegement:(NSString *)segment{
    UnitySendMessage(IRONSOURCE_EVENTS, "onSegmentReceived", MakeStringCopy(segment));
}

- (NSString *)parseErrorToEvent:(NSError *)error{
    if (error){
        NSString* codeStr =  [NSString stringWithFormat:@"%ld", (long)[error code]];
        
        NSDictionary *dict = @{@"error_description": [error localizedDescription],
                               @"error_code": codeStr};
        
        return [self getJsonFromObj:dict];
    }
    
    return nil;
}

- (NSString *)getJsonFromObj:(id)obj {
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:obj options:0 error:&error];
    
    if (!jsonData) {
        NSLog(@"Got an error: %@", error);
        return @"";
    } else {
        NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return jsonString;
    }
}

#pragma mark - C Section

#ifdef __cplusplus
extern "C" {
#endif
    
    void CFSetPluginData(const char *pluginType, const char *pluginVersion, const char *pluginFrameworkVersion){
        [[iOSBridge start] setPluginDataWithType:GetStringParam(pluginType) pluginVersion:GetStringParam(pluginVersion) pluginFrameworkVersion:GetStringParam(pluginFrameworkVersion)];
    }
    
    void CFSetAge(int age){
        [[iOSBridge start] setAge:age];
    }
    
    void CFSetGender(const char *gender){
        [[iOSBridge start] setGender:GetStringParam(gender)];
    }
    
    void CFSetMediationSegment(const char *segment){
        [[iOSBridge start] setMediationSegment:GetStringParam(segment)];
    }
    
    const char *CFGetAdvertiserId(){
        return [[iOSBridge start] getAdvertiserId];
    }
    
    void CFValidateIntegration(){
        [[iOSBridge start] validateIntegration];
    }
    
    void CFShouldTrackNetworkState(bool flag){
        [[iOSBridge start] shouldTrackNetworkState:flag];
    }
    
    bool CFSetDynamicUserId(char *dynamicUserId){
        return [[iOSBridge start] setDynamicUserId:GetStringParam(dynamicUserId)];
    }
    
    void CFSetAdaptersDebug(bool enabled){
        [[iOSBridge start] setAdaptersDebug:enabled];
    }
    
    void CFSetUserId(char *userId){
        return [[iOSBridge start] setUserId:GetStringParam(userId)];
    }
    
    void CFSetConsent (bool consent) {
        [[iOSBridge start] setConsent:consent];
    }
    
    void CFSetMetaData (char *key, char *value) {
        [[iOSBridge start] setMetaDataWithKey:GetStringParam(key) value:GetStringParam(value)];
    }
    
#pragma mark Init SDK
    
    void CFInit(const char *appKey){
        [[iOSBridge start] initWithAppKey:GetStringParam(appKey)];
    }
    
    void CFInitWithAdUnits(const char *appKey, const char *adUnits[]){
        NSMutableArray *adUnitsArray = [NSMutableArray new];
        
        if(adUnits != nil ) {
            int i = 0;
            
            while (adUnits[i] != nil) {
                [adUnitsArray addObject: [NSString stringWithCString: adUnits[i] encoding:NSASCIIStringEncoding]];
                i++;
            }
            
            [[iOSBridge start] initWithAppKey:GetStringParam(appKey) adUnits:adUnitsArray];
        }
    }
    
    void CFInitISDemandOnly(const char *appKey, const char *adUnits[]){
        NSMutableArray *adUnitsArray = [NSMutableArray new];
        
        if(adUnits != nil ) {
            int i = 0;
            
            while (adUnits[i] != nil) {
                [adUnitsArray addObject: [NSString stringWithCString: adUnits[i] encoding:NSASCIIStringEncoding]];
                i++;
            }
            [[iOSBridge start] initISDemandOnly:GetStringParam(appKey) adUnits:adUnitsArray];
        }
    }
    
#pragma mark RewardedVideo API
    void CFShowRewardedVideo(){
        [[iOSBridge start] showRewardedVideo];
    }
    
    void CFShowRewardedVideoWithPlacementName(char *placementName){
        [[iOSBridge start] showRewardedVideoWithPlacement:GetStringParam(placementName)];
    }
    
    const char *CFGetPlacementInfo(char *placementName){
        return [[iOSBridge start] getPlacementInfo:GetStringParam(placementName)];
    }
    
    bool CFIsRewardedVideoAvailable(){
        return [[iOSBridge start] isRewardedVideoAvailable];
    }
    
    bool CFIsRewardedVideoPlacementCapped(char *placementName){
        return [[iOSBridge start] isRewardedVideoPlacementCapped:GetStringParam(placementName)];
    }
    
    void CFSetRewardedVideoServerParameters(char *jsonString) {
        NSData *data = [GetStringParam(jsonString) dataUsingEncoding:NSUTF8StringEncoding];
        if (!data) {
            return;
        }
        
        NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:nil];
        if (dict) {
            [[iOSBridge start] setRewardedVideoServerParameters:dict];
        }
    }
    
    void CFClearRewardedVideoServerParameters() {
        [[iOSBridge start] clearRewardedVideoServerParameters];
    }
    
#pragma mark RewardedVideo DemandOnly API
    
    void CFShowISDemandOnlyRewardedVideo(char * instanceId){
        [[iOSBridge start] showISDemandOnlyRewardedVideo:GetStringParam(instanceId)];
    }
    
    void CFLoadISDemandOnlyRewardedVideo(char * instanceId) {
        [[iOSBridge start] loadISDemandOnlyRewardedVideo:GetStringParam(instanceId)];
    }
    
    bool CFIsDemandOnlyRewardedVideoAvailable(char * instanceId) {
        return [[iOSBridge start] isDemandOnlyRewardedVideoAvailable:GetStringParam(instanceId)];
    }
    
#pragma mark Interstitial API
    
    void CFLoadInterstitial(){
        [[iOSBridge start] loadInterstitial];
    }
    
    void CFShowInterstitial(){
        [[iOSBridge start] showInterstitial];
    }
    
    void CFShowInterstitialWithPlacementName(char *placementName){
        [[iOSBridge start] showInterstitialWithPlacement:GetStringParam(placementName)];
    }
    
    bool CFIsInterstitialReady(){
        return [[iOSBridge start] isInterstitialReady];
    }
    
    bool CFIsInterstitialPlacementCapped(char *placementName){
        return [[iOSBridge start] isInterstitialPlacementCapped:GetStringParam(placementName)];
    }
    
#pragma mark Interstitial DemandOnly API
    
    void CFLoadISDemandOnlyInterstitial(char * instanceId) {
        [[iOSBridge start] loadISDemandOnlyInterstitial:GetStringParam(instanceId)];
    }
    
    void CFShowISDemandOnlyInterstitial(char * instanceId) {
        [[iOSBridge start] showISDemandOnlyInterstitial:GetStringParam(instanceId)];
        
    }
    
    bool CFIsDemandOnlyInterstitialReady(char * instanceId) {
        return [[iOSBridge start] isISDemandOnlyInterstitialReady:GetStringParam(instanceId)];
    }
    
#pragma mark Offerwall API
    
    void CFShowOfferwall(){
        [[iOSBridge start] showOfferwall];
    }
    
    void CFShowOfferwallWithPlacementName(char *placementName){
        [[iOSBridge start] showOfferwallWithPlacement:GetStringParam(placementName)];
    }
    
    void CFGetOfferwallCredits(){
        [[iOSBridge start] getOfferwallCredits];
    }
    
    bool CFIsOfferwallAvailable(){
        return [[iOSBridge start] isOfferwallAvailable];
    }
    
#pragma mark Banner API
    
    void CFLoadBanner(char* description, int width, int height, int position, char* placementName){
        [[iOSBridge start] loadBanner:GetStringParam(description) width:width height:height position:position placement:GetStringParam(placementName)];
    }
    
    void CFDestroyBanner (){
        [[iOSBridge start] destroyBanner];
    }
    
    void CFDisplayBanner (){
        [[iOSBridge start] displayBanner];
    }
    
    void CFHideBanner (){
        [[iOSBridge start] hideBanner];
    }
    
    bool CFIsBannerPlacementCapped (char *placementName){
        return [[iOSBridge start] isBannerPlacementCapped:GetStringParam(placementName)];
    }
    
#pragma mark Segment API
    
    void CFSetSegment (char* jsonString) {
        [[iOSBridge start] setSegment:GetStringParam(jsonString)];
    }
    
#ifdef __cplusplus
}
#endif

@end


