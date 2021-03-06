using System;

namespace XSharpx {
  public struct Reducer<Q, A> {
    private readonly Func<A, A, A> op;
    private readonly Func<Q, A> unit;

    private Reducer(Func<A, A, A> op, Func<Q, A> unit) {
      this.op = op;
      this.unit = unit;
    }

    public Semigroup<A> Semigroup => Semigroup<A>.semigroup(op);

    public Func<A, A, A> Op => op;

    public Func<Q, A> Unit => unit;

    public Func<A, Q, A> Snoc {
      get {
        var t = this;
        return (a, q) => t.op(a, t.Unit(q));
      }
    }

    public Func<Q, A, A> Cons {
      get {
        var t = this;
        return (q, a) => t.op(t.Unit(q), a);
      }
    }

    public A Apply(A a1, A a2) => Semigroup.Apply(a1, a2);

    public Func<A, Func<A, A>> Curried => Semigroup.Curried;

    public Func<A, A> Curried1(A a1) => Semigroup.Curried1(a1);

    public Reducer<Q, A> Dual => Semigroup.Dual.Reducer(unit);

    public Func<A, A> Join => Semigroup.Join;

    public Reducer<Q, Pair<A, B>> Pair<B>(Reducer<Q, B> s) {
      var t = this;
      return Semigroup.Pair(s.Semigroup).Reducer<Q>(q => t.unit(q).And(s.unit(q)));
    }

    public Reducer<Q, Option<A>> Option {
      get {
        var t = this;
        return Semigroup.Option.Reducer<Q>(q => t.unit(q).Some());
      }
    }

    public Reducer<Q, B> XSelect<B>(Func<A, B> f, Func<B, A> g) {
      var t = this;
      return Semigroup.XSelect(f, g).Reducer<Q>(q => f(t.unit(q)));
    }

    public static Reducer<Q, A> reducer(Func<A, A, A> op, Func<Q, A> unit) =>
      new Reducer<Q, A>(op, unit);

    public static Reducer<A, List<A>> List => Semigroup<A>.List.Reducer<A>(a => a.ListValue());

  }
}
