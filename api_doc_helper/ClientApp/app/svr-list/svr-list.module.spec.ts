import { SvrListModule } from './svr-list.module';

describe('SvrListModule', () => {
  let svrListModule: SvrListModule;

  beforeEach(() => {
    svrListModule = new SvrListModule();
  });

  it('should create an instance', () => {
    expect(svrListModule).toBeTruthy();
  });
});
