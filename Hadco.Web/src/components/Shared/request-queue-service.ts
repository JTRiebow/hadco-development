import * as angular from 'angular';
    
angular
    .module('sharedModule')
    .service('RequestQueueService', getService());

function getService() {
    class RequestQueueService {
        private queuePrototype = {
            start(this: IQueue) {
                if (this._queue.length) {
                    this._queue.forEach(this.sendRequest.bind(this));
                }
                else {
                    this._resolvePromise();
                }
            },
            sendRequest(this: IQueue, r, i) {
                r()
                    .then(res => {
                        this.sendSuccess(res);
                        this.result.success.push(res);
                        
                        this.advanceQueue(r, res, i);
                    })
                    .catch(err => {
                        this.sendError(err);
                        this.result.failure.push(err);
                        
                        this.advanceQueue(r, err, i);
                    });
            },
            advanceQueue(this: IQueue, r, result, allI) {
                this._queue = this._queue.filter(req => req != r);
                
                this.result.all[allI] = result;
                
                if (this._currentIndex < this._requests.length) {
                    const nextRequest = this._requests[this._currentIndex];
                    
                    this._queue.push(nextRequest);
                    
                    this.sendRequest(nextRequest, this._currentIndex);
                    
                    this._currentIndex++;
                }
                else if (!this._queue.length) {
                    this._resolvePromise();
                }
            },
        } as IQueuePrototype;
        
        public create(sendRequests: RequestFunction[], options = {} as IRequestQueueOptions) {
            const { queueLength = 10 } = options;
            
            const queue = this.createQueueObject(sendRequests, queueLength);
            
            const resultPromise = this.createPromise(queue);
            
            queue.start();
            
            return resultPromise;
        }
        
        private createQueueObject(requests, queueLength: number) {
            const queue = Object.assign(Object.create(this.queuePrototype), {
                _resolvePromise: null,
                _queue: requests.slice(0, queueLength),
                _successHandlers: [],
                _failureHandlers: [],
                _currentIndex: queueLength,
                _requests: requests,
                result: {
                    success: [],
                    failure: [],
                    all: [],
                },
                sendSuccess(result) {
                    queue._successHandlers.forEach(h => h(result));
                },
                sendError(err) {
                    queue._failureHandlers.forEach(h => h(err));
                },
            }) as IQueue;
            
            return queue;
        }
        
        private createPromise(queue: IQueue) {
            const promise = new Promise((res, rej) => {
                queue._resolvePromise = () => res(queue.result);
            }) as IRequestQueue;
            
            promise.subscribe = (success?, failure?) => {
                if (typeof success == 'function') {
                    queue._successHandlers.push(success);
                }
                
                if (typeof failure == 'function') {
                    queue._failureHandlers.push(failure);
                }
                
                return promise;
            };
            
            promise.unsubscribe = (success?, failure?) => {
                queue._successHandlers = queue._successHandlers.filter(h => h != success);
                queue._failureHandlers = queue._failureHandlers.filter(h => h != failure);
                
                return promise;
            };
            
            return promise;
        }
    }
    
    RequestQueueService.$inject = [];
    
    return RequestQueueService;
}

interface IRequestQueueOptions {
    queueLength?: number;
}

interface IQueueResult<T> {
    success: T[],
    failure: any[],
    all: (any|T)[],
}

interface IRequestQueue<T = any> extends Promise<IQueueResult<T>> {
    subscribe(success?: (...args) => void, failure?: (...args) => void): IRequestQueue<T>;
    unsubscribe(success?: (...args) => void, failure?: (...args) => void): IRequestQueue<T>;
}

interface IRequestQueueService {
    create<T = any>(sendRequests: Array<(<R = any>() => ng.IPromise<R>)>, options?: IRequestQueueOptions): IRequestQueue<T>;
}

interface IQueuePrototype {
    start(): void;
    sendRequest(r: (...args) => ng.IPromise<any>, i: number): void;
    advanceQueue(r: (...args) => ng.IPromise<any>, result, allI: number): void;
}

interface IQueue<T = any> extends IQueuePrototype {
    result: IQueueResult<T>;
    sendSuccess(result): void;
    sendError(err): void;
    _resolvePromise: () => void;
    _queue: RequestFunction[];
    _successHandlers: Function[];
    _failureHandlers: Function[];
    _currentIndex: number;
    _requests: RequestFunction[];
}

type RequestFunction = <T = any>() => ng.IPromise<T>;

export {
    IRequestQueue,
    IRequestQueueOptions,
    IRequestQueueService,
    IQueueResult,
    IQueue,
    RequestFunction,
};