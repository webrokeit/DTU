dtmc

module client1
  state1 : [0..1] init 0; // State of the job (inactive/active)
  task1  : [0..5] init 0; // Length of the job
  
  [create1] state1=0 -> 0.2:(state1'=1) & (task1'=1) + 0.2 :(state1'=1) & (task1'=2) + 0.2 :(state1'=1) & (task1'=3) + 0.2: (state1'=1) & (task1'=4) + 0.2:(state1'=1) & (task1'=5);

  // Serve the job
  [serve1] state1=1 & task1>0 -> (task1'=task1-1);

  // Complete the job
  [finish1] state1=1 & task1=0 -> (state1'=0);

endmodule

module client2 = client1 [state1=state2,
                          task1=task2,
                          create1=create2,
                          serve1=serve2,
                          finish1=finish2 ]
endmodule

module client3 = client1 [state1=state3,
			  task1=task3,
			  create1=create3,
			  serve1=serve3,
			  finish1=finish3 ]
endmodule

module client4 = client1 [state1=state4,
			  task1=task4,
			  create1=create4,
			  serve1=serve4,
			  finish1=finish4 ]
endmodule

module client5 = client1 [state1=state5,
			  task1=task5,
			  create1=create5,
			  serve1=serve5,
			  finish1=finish5 ]
endmodule

module client6 = client1 [state1=state6,
			  task1=task6,
			  create1=create6,
			  serve1=serve6,
			  finish1=finish6 ]
endmodule

module client7 = client1 [state1=state7,
			  task1=task7,
			  create1=create7,
			  serve1=serve7,
			  finish1=finish7 ]
endmodule

module client8 = client1 [state1=state8,
			  task1=task8,
			  create1=create8,
			  serve1=serve8,
			  finish1=finish8 ]
endmodule

module scheduler
  job1 : bool init false; // Is there a job from client1?
  job2 : bool init false; // Is there a job from client2?
  job3 : bool init false; // Is there a job from client3?
  job4 : bool init false;
  job5 : bool init false;
  job6 : bool init false;
  job7 : bool init false;
  job8 : bool init false;
  ticket1 : [0..5] init 0; // Number of tickets for job1
  ticket2 : [0..5] init 0; // Number of tickets for job2
  ticket3 : [0..5] init 0; // Number of tickets for job3
  ticket4 : [0..5] init 0;
  ticket5 : [0..5] init 0;
  ticket6 : [0..5] init 0;
  ticket7 : [0..5] init 0;
  ticket8 : [0..5] init 0;

  // Record that there is a waiting job
  [create1] job1=false -> (job1'=true) & (ticket1'=5-task1);
  [create2] job2=false -> (job2'=true) & (ticket2'=5-task2);
  [create3] job3=false -> (job3'=true) & (ticket3'=5-task3);
  [create4] job4=false -> (job4'=true) & (ticket4'=5-task4);
  [create5] job5=false -> (job5'=true) & (ticket5'=5-task5);
  [create6] job6=false -> (job6'=true) & (ticket6'=5-task6);
  [create7] job7=false -> (job7'=true) & (ticket7'=5-task7);
  [create8] job8=false -> (job8'=true) & (ticket8'=5-task7);



  // Serve the jobs
  [serve1] job1=true & ticket1>=ticket2 & ticket1>=ticket3 & ticket1>=ticket4 & ticket1>=ticket5 & ticket1>=ticket6 & ticket1>=ticket7 & ticket1>=ticket8 -> true;
  [serve2] job2=true & ticket2>=ticket1 & ticket2>=ticket3 & ticket2>=ticket4 & ticket2>=ticket5 & ticket2>=ticket6 & ticket2>=ticket7 & ticket2>=ticket8-> true;
  [serve3] job3=true & ticket3>=ticket1 & ticket3>=ticket2 & ticket3>=ticket4 & ticket3>=ticket5 & ticket3>=ticket6 & ticket3>=ticket7 & ticket3>=ticket8-> true;
  [serve4] job4=true & ticket4>=ticket2 & ticket4>=ticket3 & ticket4>=ticket1 & ticket4>=ticket5 & ticket4>=ticket6 & ticket4>=ticket7 & ticket4>=ticket8-> true;
  [serve5] job5=true & ticket5>=ticket1 & ticket5>=ticket3 & ticket5>=ticket4 & ticket5>=ticket2 & ticket5>=ticket6 & ticket5>=ticket7 & ticket5>=ticket8-> true;
  [serve6] job6=true & ticket6>=ticket1 & ticket6>=ticket2 & ticket6>=ticket4 & ticket6>=ticket5 & ticket6>=ticket3 & ticket6>=ticket7 & ticket6>=ticket8-> true;        
  [serve7] job7=true & ticket7>=ticket1 & ticket7>=ticket2 & ticket7>=ticket4 & ticket7>=ticket5 & ticket7>=ticket3 & ticket7>=ticket6 & ticket7>=ticket8-> true;        
  [serve8] job8=true & ticket8>=ticket1 & ticket8>=ticket2 & ticket8>=ticket4 & ticket8>=ticket5 & ticket8>=ticket3 & ticket8>=ticket6 & ticket8>=ticket7-> true;        


  // Complete any job that has finished
  [finish1] job1=true -> (job1'=false) & (ticket1'=0);
  [finish2] job2=true -> (job2'=false) & (ticket2'=0);
  [finish3] job3=true -> (job3'=false) & (ticket3'=0);
  [finish4] job4=true -> (job4'=false) & (ticket4'=0);
  [finish5] job5=true -> (job5'=false) & (ticket5'=0);
  [finish6] job6=true -> (job6'=false) & (ticket6'=0);
  [finish7] job7=true -> (job7'=false) & (ticket7'=0);
  [finish8] job8=true -> (job8'=false) & (ticket8'=0);

endmodule

module monitor
  finished : bool init false;

  [finish1] finished=false -> (finished'=true);
  [finish1] finished=true -> (finished'=true);

endmodule

system
  scheduler || client1 || client2 || client3 || monitor || client4 || client5 || client6 || client7 || client8
endsystem

