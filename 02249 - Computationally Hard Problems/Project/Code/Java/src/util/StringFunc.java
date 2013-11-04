package util;

public class StringFunc {

	public static int toInt(String parse){
		return toInt(parse, 0);
	}
	
	public static int toInt(String parse, int fallback){
		try{
			return Integer.parseInt(parse, 10);
		}catch(NumberFormatException nfe){
			return fallback;
		}
		
	}

}
