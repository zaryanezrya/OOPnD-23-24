/*

Base a = new Concrete2();


void MyMethod(Base o) {
    Resource r1 = new Resource1();
    Resource r2 = new Resource2();
    try {
        a.f(r1, r2);
    } cattch (Exception ex) {
        // ...
    } finally {
        r2.Dispose();
        r1.Dispose();
    }

    using(Resource r1 = new Resource1(), Resource r2 = new Resource2()){
        try {
            a.f(r1, r2);
        } cattch (Exception ex) {
        // ...
        } 
    }
}

// -------------

BlockingCollection q = new BlockingCollection<ICommand>()

while(!stop) {
    var cmd = q.Take();
    try {
        cmd.Exeucte();
    } catch (Exception ex) {
        IoC.Resolve<ICommand>("Exception.Handler.Handle", cmd, ex).Execute();
    }
}

IDict<Type, IDict<Type, Func<ICommand, Exception, ICommand>>> handlerTree;

Func<ICommand, Exception, ICommand> handler = (cmd, ex) => {
    Type ct = cmd.GetType();
    Type ext = ex.GetType();

    return handlerTree[ct][ext](cmd, ex);
}


MoveCommand, GetPositionException -> Handler1
MoveCommand, SetPositionException -> Handler2

*, IOException -> IOExHandler

ConcreteCommand, * -> ConcreteExHandler

*, * -> DefaultHandler

*/