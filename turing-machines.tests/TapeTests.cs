namespace turing_machines.tests;

public class TapeTests
{
    [Fact]
    public void SetValue_WillPutCharOnCurrentIndex()
    {
        var tape = new Tape();
        tape.SetValue('x');
        tape.MoveRight();
        tape.SetValue('y');

        var tapeValues = tape.ToArray();
        Assert.Equal('x', tapeValues[0]);
        Assert.Equal('y', tapeValues[1]);
    }
    
    [Fact]
    public void SetValue_WillPutCharOnCurrentIndex_Negative()
    {
        var tape = new Tape();
        tape.MoveLeft();
        tape.SetValue('x');
        tape.MoveLeft();
        tape.SetValue('y');

        var tapeValues = tape.ToArray();
        //negative is being "printed" in reverse order (most left is being first)
        Assert.Equal('y', tapeValues[0]);
        Assert.Equal('x', tapeValues[1]);
    }
    
    [Fact]
    public void SetValue_WillPutCharOnCurrentIndex_Mixed()
    {
        var tape = new Tape();
        tape.SetValue('x');
        tape.MoveLeft();
        tape.SetValue('y');

        var tapeValues = tape.ToArray();
        
        Assert.Equal('y', tapeValues[0]);
        Assert.Equal('x', tapeValues[1]);
    }
    
    [Fact]
    public void CurrentIndex_WillMoveCorrectly()
    {
        var tape = new Tape();
        Assert.Equal(0, tape.CurrentIndex);
        tape.MoveRight();
        Assert.Equal(1, tape.CurrentIndex);
        tape.MoveRight();
        Assert.Equal(2, tape.CurrentIndex);
    }
    
    [Fact]
    public void CurrentIndex_WillMoveCorrectly_Negative()
    {
        var tape = new Tape();
        Assert.Equal(0, tape.CurrentIndex);
        tape.MoveLeft();
        //Will stay in place as most left is still being beginning of our tape - it just getting longer
        Assert.Equal(0, tape.CurrentIndex);
        tape.MoveLeft();
        Assert.Equal(0, tape.CurrentIndex);
    }
    
    [Fact]
    public void CurrentIndex_WillMoveCorrectly_Mixed()
    {
        var tape = new Tape();
        Assert.Equal(0, tape.CurrentIndex);
        tape.MoveRight();
        tape.MoveRight();
        Assert.Equal(2, tape.CurrentIndex);
        tape.MoveLeft();
        tape.MoveLeft();
        Assert.Equal(0, tape.CurrentIndex);
    }
    
    [Fact]
    public void Tape_WillProduceJointArrayCorrectly_ForPositiveAndNegativePart()
    {
        var tape = new Tape();
        tape.SetValue('x');
        //  V
        // |x|
        tape.MoveRight();
        tape.SetValue('y');
        //    V
        // |x|y|
        tape.MoveLeft();
        tape.MoveLeft();
        tape.SetValue('k');
        //  V
        // |k|x|y|
        tape.MoveLeft();
        tape.SetValue('l');
        //  V    
        // |l|k|x|y|
        tape.MoveRight();
        tape.MoveRight();
        tape.SetValue('m');
        //      V
        // |l|k|m|y|
        
        var expected = new []{'l', 'k', 'm', 'y'};

        var result = tape.ToArray();
        Assert.Equal(expected, result);
    }
}